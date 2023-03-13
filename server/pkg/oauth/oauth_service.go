package oauth

import (
	"context"
	"encoding/json"
	"errors"
	"fmt"
	"io"
	"net/http"

	"github.com/charmbracelet/log"
	"github.com/novaiiee/serenity/config"
	"golang.org/x/oauth2"

	"github.com/google/go-github/github"
	google "google.golang.org/api/oauth2/v2"
	"google.golang.org/api/option"
)

type OauthService interface {
	HandleExternalLogin(w http.ResponseWriter, r *http.Request, config *oauth2.Config, oauthStateString string) string
	HandleExternalCallback(w http.ResponseWriter, r *http.Request, cfg *config.Config, oauthConfig *oauth2.Config) (*UserInfo, error)
}

type oauthService struct{}

func NewOauthService() OauthService {
	return &oauthService{}
}

func (s *oauthService) HandleExternalLogin(w http.ResponseWriter, r *http.Request, config *oauth2.Config, oauthStateString string) string {
	return config.AuthCodeURL(oauthStateString)
}

func (s *oauthService) HandleExternalCallback(w http.ResponseWriter, r *http.Request, cfg *config.Config, oauthConfig *oauth2.Config) (*UserInfo, error) {
	token, err := s.exchangeToken(r.Context(), r.FormValue("state"), r.FormValue("code"), cfg.OauthStateString, oauthConfig)

	if err != nil {
		log.Error(err.Error())
		return nil, err
	}

	if oauthConfig == cfg.GoogleOauthConfig {
		userInfo, err := s.getGoogleUserInfo(r.Context(), oauthConfig, token)

		if err != nil {
			log.Error(err.Error())
			return nil, err
		}

		return &UserInfo{Email: userInfo.Email, DisplayName: userInfo.GivenName, Avatar: userInfo.Picture}, nil
	}

	if oauthConfig == cfg.GithubOauthConfig {
		userInfo, err := s.getGithubUserInfo(r.Context(), oauthConfig, token)

		if err != nil {
			log.Error(err.Error())
			return nil, err
		}

		return &UserInfo{Email: userInfo.GetEmail(), DisplayName: userInfo.GetName(), Avatar: userInfo.GetAvatarURL()}, nil
	}

	if oauthConfig == cfg.DiscordOauthConfig {
		userInfo, err := s.getDiscordUserInfo(r.Context(), oauthConfig, token)

		if err != nil {
			log.Error(err.Error())
			return nil, err
		}

		return &UserInfo{Email: userInfo.Email, DisplayName: userInfo.Username, Avatar: userInfo.Avatar}, nil
	}

	return nil, errors.New("unknown error with oauth")
}

func (s *oauthService) exchangeToken(ctx context.Context, state, code, oauthStateString string, config *oauth2.Config) (*oauth2.Token, error) {
	if state != oauthStateString {
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

func (s *oauthService) getDiscordUserInfo(ctx context.Context, config *oauth2.Config, token *oauth2.Token) (*DiscordUser, error) {
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

	var user *DiscordUser
	if err := json.Unmarshal(data, &user); err != nil {
		return nil, fmt.Errorf("could not unmarshal user data: %s", err.Error())
	}

	user.Avatar = fmt.Sprintf("https://cdn.discordapp.com/avatars/%s/%s", user.Id, user.Avatar)
	return user, nil
}
