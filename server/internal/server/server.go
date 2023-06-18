package server

import (
	"context"
	"fmt"
	"net/http"
	"os"

	"github.com/pkg/errors"

	"github.com/go-chi/chi/v5"
	"github.com/go-chi/cors"
	"github.com/go-chi/docgen"
	"github.com/go-chi/render"
	"github.com/ironstar-io/chizerolog"
	"github.com/rs/zerolog"
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
	log *zerolog.Logger
	cfg *config.Config
	db  *sqlx.DB
	r   *chi.Mux
}

func NewServer(log *zerolog.Logger, cfg *config.Config) *server {
	return &server{cfg: cfg, log: log}
}

func (s *server) Run(docs *string) {
	r := chi.NewRouter()

	if true == false {
		r.Use(chizerolog.LoggerMiddleware(s.log))
	}

	r.Use(cors.Handler(cors.Options{
		AllowedOrigins:   []string{"*"},
		AllowedMethods:   []string{"GET", "PUT", "POST", "DELETE"},
		AllowCredentials: true,
	}))

	r.Use(render.SetContentType(render.ContentTypeJSON))

	db, err := database.InitDatabase(s.cfg)

	if err == nil {
		s.log.Fatal().Stack().Err(err).Msg("")
	}

	s.log.Info().Msg("Successfully connected to the Database")

	s.r = r
	s.db = db

	ur := user.NewUserRepository(s.log, db)
	uar := account.NewUserAccountRepository(s.log, db)

	oauthService := auth.NewOauthService(s.log, s.cfg)
	userService := user.NewUserService(s.log)
	authService := auth.NewAuthService(s.log, s.cfg, ur, uar)

	uh := user.NewUserHandler(s.log, s.cfg, userService, ur)
	ah := auth.NewAuthHandler(s.log, s.cfg, oauthService, authService)

	r.Route("/auth", auth.MapAuthRoutes(ah))
	r.Route("/users", user.MapUserRoutes(uh))

	r.Mount("/swagger", httpSwagger.WrapHandler)

	if *docs == "json" {
		s.log.Info().Msg("Generating JSON docs")
		s.GenerateJSONRoutes(r)
	}

	port := ":" + s.cfg.Port
	s.log.Info().Msg(fmt.Sprintf("Running the server on port %v", port))

	go func() {
		if err := http.ListenAndServe("localhost"+port, r); err != nil {
			s.log.Fatal().Stack().Msg(fmt.Sprintf("There was an error: %v", err))
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
		s.log.Fatal().Stack().Err(err).Msg("")
	}

	f, err := os.Create("routes.json")
	if err != nil {
		s.log.Fatal().Stack().Err(err).Msg("")
	}

	defer f.Close()
	text := docgen.JSONRoutesDoc(r)

	if _, err = f.Write([]byte(text)); err != nil {
		s.log.Fatal().Stack().Err(err).Msg("")
	}
}
