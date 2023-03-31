package auth

import (
	"context"
	"net/http"

	"github.com/novaiiee/serenity/internal/domain"
	"golang.org/x/oauth2"
)

type OauthService interface {
	HandleExternalLogin(config *oauth2.Config) string
	HandleExternalCallback(r *http.Request, oauthConfig *oauth2.Config) (*domain.UserInfo, error)
}

type AuthService interface {
	Login(ctx context.Context, info *domain.UserInfo) (int, error)
}
