package services

import (
	"context"
	"encoding/json"
	"errors"
	"fmt"
	"io"
	"net/http"

	"github.com/charmbracelet/log"
	"github.com/novaiiee/serenity/config"
	"github.com/novaiiee/serenity/internal/auth"
	"github.com/novaiiee/serenity/internal/domain"
	"golang.org/x/oauth2"

	"github.com/google/go-github/github"
	google "google.golang.org/api/oauth2/v2"
	"google.golang.org/api/option"
)

type oauthService struct{
  logger *log.Logger
  cfg *config.Config
}

func NewOauthService(logger *log.Logger, cfg *config.Config) auth.OauthService {
	return &oauthService{logger: logger, cfg: cfg}
}

func (s *oauthService) HandleExternalLogin(config *oauth2.Config) string {
	return config.AuthCodeURL(s.cfg.OauthStateString)
}

func (s *oauthService) HandleExternalCallback(r *http.Request, oauthConfig *oauth2.Config) (*domain.UserInfo, error) {
	token, err := s.exchangeToken(r.Context(), r.FormValue("state"), r.FormValue("code"), oauthConfig)

	if err != nil {
		s.logger.Error(err.Error())
		return nil, err
	}

	if oauthConfig == s.cfg.GoogleOauthConfig {
		userInfo, err := s.getGoogleUserInfo(r.Context(), oauthConfig, token)

		if err != nil {
			s.logger.Error(err.Error())
			return nil, err
		}

		return &domain.UserInfo{Email: userInfo.Email, DisplayName: userInfo.GivenName, Avatar: userInfo.Picture, AccessToken: token, Provider: "google"}, nil
	}

	if oauthConfig == s.cfg.GithubOauthConfig {
		userInfo, err := s.getGithubUserInfo(r.Context(), oauthConfig, token)

		if err != nil {
			s.logger.Error(err.Error())
			return nil, err
		}

		return &domain.UserInfo{Email: userInfo.GetEmail(), DisplayName: userInfo.GetName(), Avatar: userInfo.GetAvatarURL(), AccessToken: token, Provider: "github"}, nil
	}

	if oauthConfig == s.cfg.DiscordOauthConfig {
		userInfo, err := s.getDiscordUserInfo(r.Context(), oauthConfig, token)

		if err != nil {
			s.logger.Error(err.Error())
			return nil, err
		}

		return &domain.UserInfo{Email: userInfo.Email, DisplayName: userInfo.Username, Avatar: userInfo.Avatar, AccessToken: token, Provider: "discord"}, nil
	}

	return nil, errors.New("unknown error with oauth")
}

func (s *oauthService) exchangeToken(ctx context.Context, state, code string, config *oauth2.Config) (*oauth2.Token, error) {
	if state != s.cfg.OauthStateString {
		return nil, errors.New("invalid oauth state")
	}

	token, err := config.Exchange(ctx, code)
	if err != nil {
		return nil, fmt.Errorf("code exchange failed: %s", err.Error())
	}

	return token, nil
}

func (s *oauthService) getGoogleUserInfo(ctx context.Context, config *oauth2.Config, token *oauth2.Token) (*google.Userinfo, error) {
	oauth2Service, err := google.NewService(ctx, option.WithTokenSource(config.TokenSource(ctx, token)))

	if err != nil {
		return nil, fmt.Errorf("could not initialize google service: %s", err.Error())
	}

	req := oauth2Service.Userinfo.Get()
	info, err := req.Do()

	if err != nil {
		return nil, fmt.Errorf("could not get user info: %s", err.Error())
	}

	return info, nil
}

func (s *oauthService) getGithubUserInfo(ctx context.Context, config *oauth2.Config, token *oauth2.Token) (*github.User, error) {
	oauthClient := config.Client(ctx, token)
	client := github.NewClient(oauthClient)

	user, _, err := client.Users.Get(ctx, "")

	if err != nil {
		return nil, err
	}

	return user, nil
}

func (s *oauthService) getDiscordUserInfo(ctx context.Context, config *oauth2.Config, token *oauth2.Token) (*domain.DiscordUser, error) {
	req, _ := http.NewRequestWithContext(ctx, "GET", "https://discord.com/api/users/@me", nil)
	req.Header.Set("Authorization", fmt.Sprintf("Bearer %s", token.AccessToken))
	res, err := http.DefaultClient.Do(req)

	if err != nil {
		return nil, fmt.Errorf("could not get user info: %s", err.Error())
	}

	defer res.Body.Close()
	data, err := io.ReadAll(res.Body)

	if err != nil {
		return nil, fmt.Errorf("could not get data from request: %s", err.Error())
	}

	var user *domain.DiscordUser
	if err := json.Unmarshal(data, &user); err != nil {
		return nil, fmt.Errorf("could not unmarshal user data: %s", err.Error())
	}

	user.Avatar = fmt.Sprintf("https://cdn.discordapp.com/avatars/%s/%s", user.Id, user.Avatar)
	return user, nil
}
