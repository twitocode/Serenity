package config

import (
	"github.com/spf13/viper"
	"golang.org/x/oauth2"

  githubOauth "golang.org/x/oauth2/github"
	googleOauth "golang.org/x/oauth2/google"
  discord "github.com/ravener/discord-oauth2"
)

type Config struct {
	GoogleID      string `mapstructure:"GOOGLE_ID"`
	GoogleSecret  string `mapstructure:"GOOGLE_SECRET"`
	DiscordID     string `mapstructure:"DISCORD_ID"`
	DiscordSecret string `mapstructure:"DISCORD_SECRET"`
	GithubID      string `mapstructure:"GITHUB_ID"`
	GithubSecret  string `mapstructure:"GITHUB_SECRET"`

	DBHost     string `mapstructure:"DB_HOST"`
	DBName     string `mapstructure:"DB_NAME"`
	DBPort     string `mapstructure:"DB_PORT"`
	DBUsername string `mapstructure:"DB_USERNAME"`
	DBPassword string `mapstructure:"DB_PASSWORD"`

	Port string `mapstructure:"PORT"`

  GoogleOauthConfig *oauth2.Config
  GithubOauthConfig *oauth2.Config
  DiscordOauthConfig *oauth2.Config

  OauthStateString string
}

func Get() (*Config, error) {
	viper.SetConfigType("env")
	viper.SetConfigName("app")
	viper.AddConfigPath(".")

	viper.AutomaticEnv()

	if err := viper.ReadInConfig(); err != nil {
		return nil, err
	}

	var cfg *Config
	if err := viper.Unmarshal(&cfg); err != nil {
		return nil, err
	}

  cfg.GoogleOauthConfig = &oauth2.Config{
		ClientID:     cfg.GoogleID,
		ClientSecret: cfg.GoogleSecret,
		Endpoint:     googleOauth.Endpoint,
		Scopes:       []string{"https://www.googleapis.com/auth/userinfo.email", "https://www.googleapis.com/auth/userinfo.profile"},
		RedirectURL:  "http://localhost:8000/auth/google/callback",
	}

	cfg.DiscordOauthConfig = &oauth2.Config{
    ClientID:     cfg.DiscordID,
		ClientSecret: cfg.DiscordSecret,
		Endpoint:     discord.Endpoint,
		Scopes:       []string{"email", "guilds", "identify"},
		RedirectURL:  "http://localhost:8000/auth/discord/callback",
	}

	cfg.GithubOauthConfig = &oauth2.Config{
    ClientID:     cfg.GithubID,
		ClientSecret:cfg.GithubSecret,
		Endpoint:     githubOauth.Endpoint,
		Scopes:       []string{"user:email", "repo"},
		RedirectURL:  "http://localhost:8000/auth/github/callback",
	}

  cfg.OauthStateString = generateOauthString()

	return cfg, nil
}

func generateOauthString() string {
  return "HelloWorld"
}