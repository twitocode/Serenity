package oauth

type UserInfo struct {
  DisplayName string
  Email string
  Avatar string
}

type DiscordUser struct {
  Id string `json:"id"`
  Username string `json:"username"`
  Avatar string `json:"avatar"`
  Email string `json:"email"`
}