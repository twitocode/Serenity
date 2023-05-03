package server

import (
	"context"
	"errors"
	"fmt"
	"net/http"
	"os"

	"github.com/go-chi/chi/v5"
	"github.com/go-chi/cors"
	"github.com/go-chi/docgen"
	"github.com/go-chi/render"
	"github.com/rs/zerolog/log"
	httpSwagger "github.com/swaggo/http-swagger"

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
}

func NewServer(cfg *config.Config) *server {
	return &server{cfg: cfg}
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
	r.Mount("/swagger", httpSwagger.WrapHandler)
	db, err := database.InitDatabase(s.cfg)

	if err != nil {
		log.Fatal().Stack().Msg(fmt.Sprintf("Database connection error: %v", err))
	}

	log.Info().Msg("Successfully connected to the Database")

	s.r = r
	s.db = db

	ur := user.NewUserRepository(db)
	uar := account.NewUserAccountRepository(db)

	oauthService := auth.NewOauthService(s.cfg)
	userService := user.NewUserService()
	authService := auth.NewAuthService(s.cfg, ur, uar)

	uh := user.NewUserHandler(s.cfg, userService, ur)
	ah := auth.NewAuthHandler(s.cfg, oauthService, authService)

	r.Route("/auth", auth.MapAuthRoutes(ah))
	r.Route("/users", user.MapUserRoutes(uh))

	if *docs == "json" {
		log.Info().Msg("Generating JSON docs")
		s.GenerateJSONRoutes(r)
	}

	port := ":" + s.cfg.Port
	log.Info().Msg(fmt.Sprintf("Running the server on port %v", port))

	go func() {
		if err := http.ListenAndServe("localhost"+port, r); err != nil {
			log.Fatal().Stack().Msg(fmt.Sprintf("There was an error: %v", err))
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
		log.Fatal().Stack().Err(err).Msg("")
	}

	f, err := os.Create("routes.json")
	if err != nil {
		log.Fatal().Stack().Err(err).Msg("")
	}

	defer f.Close()
	text := docgen.JSONRoutesDoc(r)

	if _, err = f.Write([]byte(text)); err != nil {
		log.Fatal().Stack().Err(err).Msg("")
	}
}
