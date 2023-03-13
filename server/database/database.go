package database

import (
	"fmt"

	"github.com/jmoiron/sqlx"
  "github.com/novaiiee/serenity/config"
)

func InitDatabase(cfg *config.Config) (*sqlx.DB, error) {
	connString := fmt.Sprintf("user=%s dbname=%s sslmode=disable port=%s host=%s password=%s",
		cfg.DBUsername, cfg.DBName, cfg.DBPort, cfg.DBHost, cfg.DBPassword)

	db, err := sqlx.Connect("postgres", connString)
	if err != nil {
		return nil, err
	}

	return db, nil
}
