package database

import (
	"fmt"

	"github.com/jmoiron/sqlx"
	"github.com/novaiiee/serenity/config"
)

func InitDatabase(cfg *config.Config) (*sqlx.DB, error) {
	host := "localhost"

	connString := fmt.Sprintf("user=%s dbname=%s sslmode=disable port=5432 host=%s password=%s",
		cfg.DBUser, cfg.DBName, host, cfg.DBPassword)

	db, err := sqlx.Connect("postgres", connString)
	if err != nil {
		return nil, err
	}

	return db, nil
}
