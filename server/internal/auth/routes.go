package auth

import (
	"net/http"

	"github.com/go-chi/chi/v5"
)

type AuthHandler interface {
	ExternalProvider() http.HandlerFunc
	ExternalProviderCallback() http.HandlerFunc
	LoginWithEmailPassword() http.HandlerFunc
	RegisterWithEmailPassword() http.HandlerFunc
}

func MapAuthRoutes(h AuthHandler) func(r chi.Router) {
	return func(r chi.Router) {
		r.Get("/{provider}", h.ExternalProvider())
		r.Get("/{provider}/callback", h.ExternalProviderCallback())
		r.Post("/register", h.RegisterWithEmailPassword())
		r.Post("/login", h.LoginWithEmailPassword())
	}
}
