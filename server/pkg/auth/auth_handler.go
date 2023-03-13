package auth

import (
	"net/http"

	"github.com/charmbracelet/log"
	"github.com/go-chi/chi/v5"
	"github.com/novaiiee/serenity/config"
	"github.com/novaiiee/serenity/pkg/oauth"
)

type AuthHandler struct {
	r            chi.Router
	cfg          *config.Config
	oauthService oauth.OauthService
}

//CHECK IF A USER EXISTS WITH THE EMAIL OR PROVIDER EMAIL

func NewAuthHandler(r chi.Router, cfg *config.Config, oauthService oauth.OauthService) *AuthHandler {
	h := &AuthHandler{r: r, cfg: cfg, oauthService: oauthService}

	r.Route("/auth", func(r chi.Router) {
		r.Get("/google", func(w http.ResponseWriter, r *http.Request) {
			url := oauthService.HandleExternalLogin(w, r, cfg.GoogleOauthConfig, cfg.OauthStateString)
			http.Redirect(w, r, url, http.StatusTemporaryRedirect)
		})

		r.Get("/github", func(w http.ResponseWriter, r *http.Request) {
			url := oauthService.HandleExternalLogin(w, r, cfg.GithubOauthConfig, cfg.OauthStateString)
			http.Redirect(w, r, url, http.StatusTemporaryRedirect)
		})

		r.Get("/discord", func(w http.ResponseWriter, r *http.Request) {
			url := oauthService.HandleExternalLogin(w, r, cfg.DiscordOauthConfig, cfg.OauthStateString)
			http.Redirect(w, r, url, http.StatusTemporaryRedirect)
		})

		r.Get("/google/callback", func(w http.ResponseWriter, r *http.Request) {
			userInfo, err := oauthService.HandleExternalCallback(w, r, cfg, cfg.GoogleOauthConfig)

			if err != nil {
				log.Error(err)
        w.Write([]byte(err.Error()))
				return
			}

			w.Write([]byte(userInfo.Email))
		})

		r.Get("/github/callback", func(w http.ResponseWriter, r *http.Request) {
			userInfo, err := oauthService.HandleExternalCallback(w, r, cfg, cfg.GithubOauthConfig)

			if err != nil {
				log.Error(err)
        w.Write([]byte(err.Error()))
				return
			}

			w.Write([]byte(userInfo.Email))
		})

		r.Get("/discord/callback", func(w http.ResponseWriter, r *http.Request) {
			userInfo, err := oauthService.HandleExternalCallback(w, r, cfg, cfg.DiscordOauthConfig)

			if err != nil {
				log.Error(err)
        w.Write([]byte(err.Error()))
				return
			}

			w.Write([]byte(userInfo.Email))
		})
	})

	return h
}
