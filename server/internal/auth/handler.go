package auth

import (
	"encoding/json"
	"net/http"

	"github.com/charmbracelet/log"
	"github.com/go-chi/chi/v5"
	golangJwt "github.com/golang-jwt/jwt/v5"
	"github.com/novaiiee/serenity/config"
	"github.com/novaiiee/serenity/internal/domain"
	"github.com/novaiiee/serenity/pkg/jwt"
	"github.com/novaiiee/serenity/pkg/validation"
	"golang.org/x/oauth2"
)

type authHandler struct {
	cfg    *config.Config
	os     OauthService
	as     AuthService
	logger *log.Logger
}

func NewAuthHandler(cfg *config.Config, logger *log.Logger, os OauthService, as AuthService) AuthHandler {
	return &authHandler{cfg: cfg, os: os, as: as, logger: logger}
}

func (s *authHandler) getProviderConfigs() map[string]*oauth2.Config {
	var configs = map[string]*oauth2.Config{
		"google":  s.cfg.GoogleOauthConfig,
		"github":  s.cfg.GithubOauthConfig,
		"discord": s.cfg.DiscordOauthConfig,
	}

	return configs
}

func (s *authHandler) ExternalProvider() http.HandlerFunc {
	return func(w http.ResponseWriter, r *http.Request) {
		config := s.getProviderConfigs()[chi.URLParam(r, "provider")]
		url := s.os.HandleExternalLogin(config)

		http.Redirect(w, r, url, http.StatusTemporaryRedirect)
	}
}

func (s *authHandler) ExternalProviderCallback() http.HandlerFunc {
	return func(w http.ResponseWriter, r *http.Request) {
		config := s.getProviderConfigs()[chi.URLParam(r, "provider")]
		userInfo, err := s.os.HandleExternalCallback(r, config)

		if err != nil {
			s.logger.Error(err)
			w.Write([]byte(err.Error()))
			return
		}

		id, err := s.as.ExternalLogin(r.Context(), userInfo)

		if err != nil {
			s.logger.Error(err)
			w.Write([]byte(err.Error()))
			return
		}

		token, err := jwt.GenerateJwt(s.cfg.JwtAccessSecret, golangJwt.MapClaims{
			"user": id,
		})

		if err != nil {
			s.logger.Error(err)
			w.Write([]byte(err.Error()))
			return
		}

		//Redirect with token
		json.NewEncoder(w).Encode(map[string]string{
			"token": token,
		})
	}
}

func (s *authHandler) RegisterWithEmailPassword() http.HandlerFunc {
	return func(w http.ResponseWriter, r *http.Request) {
		var body domain.RegisterUserRequest

		if err := json.NewDecoder(r.Body).Decode(&body); err != nil {
			log.Error(err)
			w.Write([]byte(err.Error()))
			return
		}

		if err := validaton.ValidateStruct(body); err != nil {
			log.Error(err)
			w.Write([]byte(err.Error()))
			return
		}

		id, err := s.as.Register(r.Context(), &body)

		if err != nil {
			log.Error(err)
			w.Write([]byte(err.Error()))
			return
		}

		token, err := jwt.GenerateJwt(s.cfg.JwtAccessSecret, golangJwt.MapClaims{
			"user": id,
		})

		if err != nil {
			s.logger.Error(err)
			w.Write([]byte(err.Error()))
			return
		}

		//Redirect with token
		json.NewEncoder(w).Encode(map[string]string{
			"token": token,
		})
	}
}

func (s *authHandler) LoginWithEmailPassword() http.HandlerFunc {
	return func(w http.ResponseWriter, r *http.Request) {
		var body domain.LoginUserRequest

		if err := json.NewDecoder(r.Body).Decode(&body); err != nil {
			log.Error(err)
			w.Write([]byte(err.Error()))
			return
		}

		if err := validaton.ValidateStruct(body); err != nil {
			log.Error(err)
			w.Write([]byte(err.Error()))
			return
		}

		id, err := s.as.Login(r.Context(), &body)

		if err != nil {
			log.Error(err)
			w.Write([]byte(err.Error()))
			return
		}

		token, err := jwt.GenerateJwt(s.cfg.JwtAccessSecret, golangJwt.MapClaims{
			"user": id,
		})

		if err != nil {
			s.logger.Error(err)
			w.Write([]byte(err.Error()))
			return
		}

		//Redirect with token
		json.NewEncoder(w).Encode(map[string]string{
			"token": token,
		})
	}
}