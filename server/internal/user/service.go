package user

import (
	"github.com/charmbracelet/log"
)

type UserService interface {
}

type userService struct {
	logger *log.Logger
}

func NewUserService(logger *log.Logger) UserService {
	return &userService{logger: logger}
}