package services

import (
	"github.com/charmbracelet/log"
	"github.com/novaiiee/serenity/internal/user"
)

type userService struct {
	logger *log.Logger
}

func NewUserService(logger *log.Logger) user.UserService {
	return &userService{logger: logger}
}