package user

import "github.com/rs/zerolog"

type UserService interface {
}
type userService struct {
	log *zerolog.Logger
}

func NewUserService(log *zerolog.Logger) UserService {
	return &userService{log: log}
}
