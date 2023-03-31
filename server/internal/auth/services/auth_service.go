package services

import (
	"context"
	"errors"
	"fmt"

	"github.com/charmbracelet/log"
	"github.com/novaiiee/serenity/config"
	"github.com/novaiiee/serenity/internal/auth"
	"github.com/novaiiee/serenity/internal/domain"
	"github.com/novaiiee/serenity/internal/user"
)

type authService struct {
	ur  user.UserRepository
	cfg *config.Config
  logger *log.Logger
}

func NewAuthService(cfg *config.Config, logger *log.Logger, ur user.UserRepository) auth.AuthService {
	return &authService{cfg: cfg, ur: ur, logger: logger}
}

func (s *authService) Login(ctx context.Context, info *domain.UserInfo) (int, error) {
	account, err := s.ur.GetAccountByEmail(ctx, info.Provider, info.Email)

	if err != nil {
		fmt.Println("GetAccountByEmail Error")
		return 0, err
	}

	if account != nil {
		user, err := s.ur.GetUserById(ctx, account.UserId)
		if err != nil {
			fmt.Println("GetUserById Error")
			return 0, err
		}

		if user == nil {
			fmt.Println("GetUserById Error")
			return 0, errors.New("User associated with account does not exist")
		}

		return user.Id, nil
	}

	user, err := s.ur.GetUserByEmail(ctx, info.Email)

	if user != nil {
		return 0, errors.New("user already exists with your providers email")
	}

	id, err := s.ur.CreateUserWithInfo(ctx, info)

	if err != nil {
		fmt.Println("CreateUserWithInfo Error")
		return 0, err
	}

	_, err = s.ur.CreateAccountWithInfo(ctx, info, id)

	if err != nil {
		fmt.Println("CreateUserWithInfo Error")
		return 0, err
	}

	return id, nil
}
