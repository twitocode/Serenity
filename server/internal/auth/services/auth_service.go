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
	"github.com/novaiiee/serenity/pkg/utils"
)

type authService struct {
	ur     user.UserRepository
	cfg    *config.Config
	logger *log.Logger
}

func NewAuthService(cfg *config.Config, logger *log.Logger, ur user.UserRepository) auth.AuthService {
	return &authService{cfg: cfg, ur: ur, logger: logger}
}

func (s *authService) ExternalLogin(ctx context.Context, info *domain.UserInfo) (int, error) {
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

func (s *authService) Register(ctx context.Context, body *domain.RegisterUserRequest) (int, error) {
	userId, err := s.ur.GetUserIdByEmail(ctx, body.Email)

	if err != nil {
		return 0, err
	}

	//If the user already exists
	if userId != 0 {
		return 0, errors.New("user already exists with email")
	}

	userId, err = s.ur.GetUserIdByDisplayName(ctx, body.DisplayName)

	if err != nil {
		return 0, err
	}

	//If the user already exists
	if userId != 0 {
		return 0, errors.New("user already exists with display_name")
	}

	hashedPass, err := utils.HashPassword(body.Password)
	if err != nil {
		return 0, err
	}

	userInfo := &domain.UserInfo{
		Email:       body.Email,
		DisplayName: body.DisplayName,
		Avatar:      "",
		Password:    hashedPass,
	}

	id, err := s.ur.CreateUserWithInfo(ctx, userInfo)
	if err != nil {
		return 0, err
	}

	return id, nil
}

func (s *authService) Login(ctx context.Context, body *domain.LoginUserRequest) (int, error) {
	user, err := s.ur.GetUserPasswordByIdentifier(ctx, body.Identifier)

  if err != nil {
    return 0, err
  }

  if user == nil {
    return 0, errors.New("user does not exist")
  }

  if !user.Password.Valid {
    return 0, errors.New("user does not have a password")
  }

	if err := utils.CompareHashAndPassword(body.Password, user.Password.String); err != nil {
    return 0, err
  }

	return user.Id, nil
}
