package user

import (
	"fmt"
	"net/http"
	"strconv"

	"github.com/charmbracelet/log"
	"github.com/go-chi/chi/v5"
)

type UserHandler struct {
	userRepository UserRepository
	r              *chi.Mux
}

func NewUserHandler(r *chi.Mux, userRepository UserRepository) {
	h := &UserHandler{userRepository: userRepository, r: r}
	r.Route("/users", func(r chi.Router) {
		r.Get("/{id}", h.GetUserById)
	})
}

func (h *UserHandler) GetUserById(w http.ResponseWriter, r *http.Request) {
	id, err := strconv.Atoi(chi.URLParam(r, "id"))
	if err != nil {
		w.Write([]byte("Not a proper Id"))
		return
	}

	user, err := h.userRepository.GetUserById(id)

	if err != nil {
		log.Error(err.Error())
		w.Write([]byte(fmt.Sprintf("No user found with the id %d", id)))
		return
	}
  
	w.Write([]byte(user.Email))
}
