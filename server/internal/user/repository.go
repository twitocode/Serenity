package user

import (
	"context"

	"github.com/novaiiee/serenity/internal/domain"
)

type UserRepository interface {
	CreateUserWithInfo(ctx context.Context, info *domain.UserInfo) (int ,error)
	GetAccountByEmail(ctx context.Context, provider string, email string) (*domain.UserOauthInfo, error)
	GetUserAccount(ctx context.Context, provider string, email string) (*domain.UserOauthInfo, error)
	GetUserById(ctx context.Context, id int) (*domain.User, error)
	GetUserIdByEmail(ctx context.Context, email string) (int, error)
	GetUserByEmail(ctx context.Context, email string) (*domain.User, error)
  CreateAccountWithInfo(ctx context.Context, info *domain.UserInfo, userId int) (int, error) 
}
