package auth

import (
	"context"
	"errors"
  
	"github.com/novaiiee/serenity/config"
	"github.com/novaiiee/serenity/internal/account"
	"github.com/novaiiee/serenity/internal/domain"
	"github.com/novaiiee/serenity/internal/user"
	"github.com/novaiiee/serenity/pkg/crypto"
)

type AuthService interface {
	ExternalLogin(ctx context.Context, info *domain.UserInfo) (int, error)
	Register(ctx context.Context, body *domain.RegisterUserRequest) (int, error)
	Login(ctx context.Context, body *domain.LoginUserRequest) (int, error)
}

type authService struct {
	ur     user.UserRepository
	uar    account.UserAccountRepository
	cfg    *config.Config
}

func NewAuthService(cfg *config.Config, ur user.UserRepository, uar account.UserAccountRepository) AuthService {
	return &authService{cfg: cfg, ur: ur, uar: uar}
}

func (s *authService) ExternalLogin(ctx context.Context, info *domain.UserInfo) (int, error) {
	account, err := s.uar.GetAccountByEmail(ctx, info.Provider, info.Email)

	if err != nil {
		return 0, err
	}

	if account != nil {
		user, err := s.ur.GetUserById(ctx, account.UserId)
		if err != nil {
			return 0, err
		}

		if user == nil {
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
		return 0, err
	}

	_, err = s.uar.CreateAccountWithInfo(ctx, info, id)

	if err != nil {
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

	hashedPass, err := crypto.HashPassword(body.Password)
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

	if err := crypto.CompareHashAndPassword(body.Password, user.Password.String); err != nil {
		return 0, err
	}

	return user.Id, nil
}
