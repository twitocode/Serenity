package http

import (
	"github.com/go-chi/chi/v5"
	"github.com/novaiiee/serenity/internal/user"
)

func MapUserRoutes(h user.Handler) func(r chi.Router) {
  return func(r chi.Router) {
    r.Get("/{id}", h.GetUserById())
  }
}
