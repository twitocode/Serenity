package domain

import "golang.org/x/oauth2"

type UserInfo struct {
	DisplayName string
	Email       string
	Avatar      string
	AccessToken *oauth2.Token
	Provider    string
}

type DiscordUser struct {
	Id       string `json:"id"`
	Username string `json:"username"`
	Avatar   string `json:"avatar"`
	Email    string `json:"email"`
}
