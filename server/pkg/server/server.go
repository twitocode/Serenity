package server

import (
	"context"
	"fmt"
	"net/http"

	"github.com/charmbracelet/log"
	"github.com/go-chi/chi/v5"
	"github.com/jmoiron/sqlx"
	_ "github.com/lib/pq"
	"github.com/novaiiee/serenity/config"
	"github.com/novaiiee/serenity/database"
	"github.com/novaiiee/serenity/pkg/auth"
	"github.com/novaiiee/serenity/pkg/oauth"
	"github.com/novaiiee/serenity/pkg/user"
)

type server struct {
	cfg *config.Config
	db  *sqlx.DB
	r   *chi.Mux
}

func NewServer(cfg *config.Config) *server {
	return &server{cfg: cfg}
}

func (s *server) Run() {
	r := chi.NewRouter()
	db, err := database.InitDatabase(s.cfg)

	if err != nil {
		log.Fatal(fmt.Sprintf("Database connection error: %v", err))
	}

	log.Info("Successfully connected to the Database")

	s.r = r
	s.db = db

	oauthService := oauth.NewOauthService()
	ur := user.NewUserRepository(db)

	user.NewUserHandler(r, ur)
	auth.NewAuthHandler(r, s.cfg, oauthService)

	port := ":" + s.cfg.Port
	log.Info(fmt.Sprintf("Running the server on port %v", port))

	go func() {
		if err := http.ListenAndServe("localhost"+port, r); err != nil {
			log.Fatal(fmt.Sprintf("There was an error: %v", err))
		}
	}()
}

func (s *server) Shutdown(ctx context.Context) error {
	if err := s.db.Close(); err != nil {
		return err
	}

	return nil
}
