package user

import (
	"github.com/go-chi/chi/v5"
)

func MapUserRoutes(h UserHandler) func(r chi.Router) {
	return func(r chi.Router) {
		r.Get("/{id}", h.GetUserById())
	}
}
