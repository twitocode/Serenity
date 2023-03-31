package config

import (
	"os"

	"golang.org/x/oauth2"

	github "github.com/google/go-github/github"
	"github.com/joho/godotenv"
	discordOauth "github.com/ravener/discord-oauth2"
	githubOauth "golang.org/x/oauth2/github"
	googleOauth "golang.org/x/oauth2/google"
	google "google.golang.org/api/oauth2/v2"
)

type Config struct {
	GoogleID      string
	GoogleSecret  string
	DiscordID     string
	DiscordSecret string
	GithubID      string
	GithubSecret  string

	DBName     string
	DBUser     string
	DBPassword string
	AppEnv     string

	Port string

	GoogleOauthConfig  *oauth2.Config
	GithubOauthConfig  *oauth2.Config
	DiscordOauthConfig *oauth2.Config

	OauthStateString string
	JwtAccessSecret  string
}

func Get() (*Config, error) {
	if err := godotenv.Load(); err != nil {
		return nil, err
	}

	cfg := &Config{
		GoogleID:         os.Getenv("GOOGLE_ID"),
		GoogleSecret:     os.Getenv("GOOGLE_SECRET"),
		GithubID:         os.Getenv("GITHUB_ID"),
		GithubSecret:     os.Getenv("GITHUB_SECRET"),
		DiscordID:        os.Getenv("DISCORD_ID"),
		DiscordSecret:    os.Getenv("DISCORD_SECRET"),
		DBName:           os.Getenv("DB_NAME"),
		DBUser:           os.Getenv("DB_USER"),
		DBPassword:       os.Getenv("DB_PASSWORD"),
		Port:             os.Getenv("PORT"),
		AppEnv:           os.Getenv("APP_ENV"),
		OauthStateString: generateOauthString(),
		JwtAccessSecret:  os.Getenv("JWT_ACCESS_SECRET"),
	}

	cfg.GoogleOauthConfig = &oauth2.Config{
		ClientID:     cfg.GoogleID,
		ClientSecret: cfg.GoogleSecret,
		Endpoint:     googleOauth.Endpoint,
		Scopes:       []string{google.UserinfoEmailScope, google.UserinfoProfileScope},
		RedirectURL:  "http://localhost:8000/auth/google/callback",
	}

	cfg.DiscordOauthConfig = &oauth2.Config{
		ClientID:     cfg.DiscordID,
		ClientSecret: cfg.DiscordSecret,
		Endpoint:     discordOauth.Endpoint,
		Scopes:       []string{discordOauth.ScopeEmail, discordOauth.ScopeIdentify, discordOauth.ScopeGuilds},
		RedirectURL:  "http://localhost:8000/auth/discord/callback",
	}

	cfg.GithubOauthConfig = &oauth2.Config{
		ClientID:     cfg.GithubID,
		ClientSecret: cfg.GithubSecret,
		Endpoint:     githubOauth.Endpoint,
		Scopes:       []string{string(github.ScopeUserEmail)},
		RedirectURL:  "http://localhost:8000/auth/github/callback",
	}

	return cfg, nil
}

func generateOauthString() string {
	return "HelloWorld"
}

// viper.SetConfigType("env")
// viper.SetConfigName(".env")
// viper.AddConfigPath(".")

// viper.AutomaticEnv()

// if err := viper.ReadInConfig(); err != nil {
// 	return nil, err
// }

// var cfg *Config
// if err := viper.Unmarshal(&cfg); err != nil {
// 	return nil, err
// }
