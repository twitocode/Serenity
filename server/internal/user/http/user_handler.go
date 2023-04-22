package http

import (
	"encoding/json"
	"fmt"
	"net/http"
	"strconv"

	"github.com/charmbracelet/log"
	"github.com/go-chi/chi/v5"
	"github.com/novaiiee/serenity/config"
	"github.com/novaiiee/serenity/internal/user"
)

type UserHandler struct {
	cfg    *config.Config
	ur     user.UserRepository
	us     user.UserService
	logger *log.Logger
}

func NewUserHandler(cfg *config.Config, logger *log.Logger, userService user.UserService, userRepository user.UserRepository) user.UserHandler {
	return &UserHandler{cfg: cfg, ur: userRepository, us: userService, logger: logger}
}

func (h *UserHandler) GetUserById() http.HandlerFunc {
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
