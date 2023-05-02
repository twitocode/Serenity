package user

import (
	"encoding/json"
	"fmt"
	"net/http"
	"strconv"

	"github.com/charmbracelet/log"
	"github.com/go-chi/chi/v5"
	"github.com/novaiiee/serenity/config"
)

type UserHandler interface {
	GetUserById() http.HandlerFunc
}


type userHandler struct {
	cfg    *config.Config
	ur     UserRepository
	us     UserService
	logger *log.Logger
}

func NewUserHandler(cfg *config.Config, logger *log.Logger, userService UserService, userRepository UserRepository) *userHandler {
	return &userHandler{cfg: cfg, ur: userRepository, us: userService, logger: logger}
}

func (h *userHandler) GetUserById() http.HandlerFunc {
	return func(w http.ResponseWriter, r *http.Request) {
		id, err := strconv.Atoi(chi.URLParam(r, "id"))
		if err != nil {
			w.Write([]byte("Not a proper Id"))
			return
		}

		user, err := h.ur.GetUserById(r.Context(), id)

		if err != nil {
			h.logger.Error(err.Error())
			w.Write([]byte(fmt.Sprintf("No user found with the id %d", id)))
			return
		}

		json.NewEncoder(w).Encode(user)
	}

}
