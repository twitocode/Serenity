package server

import (
	"context"
	"errors"
	"fmt"
	"net/http"
	"os"

	"github.com/charmbracelet/log"
	"github.com/go-chi/chi/v5"
	"github.com/go-chi/cors"
	"github.com/go-chi/docgen"
	"github.com/go-chi/render"

	"github.com/jmoiron/sqlx"
	_ "github.com/lib/pq"
	"github.com/novaiiee/serenity/config"
	database "github.com/novaiiee/serenity/db"
	"github.com/novaiiee/serenity/internal/account"
	auth "github.com/novaiiee/serenity/internal/auth"
	user "github.com/novaiiee/serenity/internal/user"
)

type server struct {
	cfg *config.Config
	db  *sqlx.DB
	r   *chi.Mux
  logger *log.Logger
}

func NewServer(logger *log.Logger, cfg *config.Config) *server {
	return &server{cfg: cfg, logger: logger}
}

func (s *server) Run(docs *string) {
	r := chi.NewRouter()

	r.Use(cors.Handler(cors.Options{
		AllowedOrigins:   []string{"*"},
		AllowedMethods:   []string{"GET", "PUT", "POST", "DELETE"},
		AllowCredentials: true,
	}))

	//r.Use(middleware.Logger)
	r.Use(render.SetContentType(render.ContentTypeJSON))
	db, err := database.InitDatabase(s.cfg)

	if err != nil {
		s.logger.Fatal(fmt.Sprintf("Database connection error: %v", err))
	}

	s.logger.Info("Successfully connected to the Database")

	s.r = r
	s.db = db

	ur := user.NewUserRepository(db)
	uar := account.NewUserAccountRepository(db)

	oauthService := auth.NewOauthService(s.logger, s.cfg)
	userService := user.NewUserService(s.logger)
	authService := auth.NewAuthService(s.cfg, s.logger, ur, uar)

	uh := user.NewUserHandler(s.cfg, s.logger, userService, ur)
	ah := auth.NewAuthHandler(s.cfg, s.logger, oauthService, authService)

	r.Route("/auth", auth.MapAuthRoutes(ah))
	r.Route("/users", user.MapUserRoutes(uh))

	if *docs == "json" {
		s.logger.Info("Generating JSON docs")
		s.GenerateJSONRoutes(r)
	}

	port := ":" + s.cfg.Port
	s.logger.Info(fmt.Sprintf("Running the server on port %v", port))

	go func() {
		if err := http.ListenAndServe("localhost"+port, r); err != nil {
			s.logger.Fatal(fmt.Sprintf("There was an error: %v", err))
		}
	}()
}

func (s *server) Shutdown(ctx context.Context) error {
	if err := s.db.Close(); err != nil {
		return err
	}

	return nil
}

func (s *server) GenerateJSONRoutes(r chi.Router) {
	if err := os.Remove("routes.json"); err != nil && !errors.Is(err, os.ErrNotExist) {
		s.logger.Fatal(err)
	}

	f, err := os.Create("routes.json")
	if err != nil {
		s.logger.Fatal(err)
	}

	defer f.Close()
	text := docgen.JSONRoutesDoc(r)

	if _, err = f.Write([]byte(text)); err != nil {
		s.logger.Fatal(err)
	}
}
