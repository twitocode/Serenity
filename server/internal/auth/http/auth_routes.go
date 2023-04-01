package http

import (
	"github.com/go-chi/chi/v5"
	"github.com/novaiiee/serenity/internal/auth"
)

func MapAuthRoutes(h auth.Handler) func(r chi.Router) {
	return func(r chi.Router) {
		r.Get("/{provider}", h.ExternalProvider())
		r.Get("/{provider}/callback", h.ExternalProviderCallback())
		r.Post("/register", h.RegisterWithEmailPassword())
		r.Post("/login", h.LoginWithEmailPassword())
	}
}
