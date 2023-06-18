package main

import (
	"context"
	"flag"
	"os"
	"os/signal"
	"syscall"
	"time"

	"github.com/novaiiee/serenity/config"
	"github.com/novaiiee/serenity/internal/server"
	"github.com/rs/zerolog"
	"github.com/rs/zerolog/pkgerrors"
)

var docs = flag.String("docs", "", "Generates routing documentation for RESTful API - markdown, json, or raml.")

func main() {
  log := zerolog.New(zerolog.ConsoleWriter{Out: os.Stdout}).With().Timestamp().Logger()
	zerolog.ErrorStackMarshaler = pkgerrors.MarshalStack

	flag.Parse()
	cfg, err := config.Get()

	if err != nil {
    log.Error().Stack().Err(err).Msg("Error with getting config")
  }

	server := server.NewServer(&log, cfg)
	server.Run(docs)

	quit := make(chan os.Signal, 1)
	signal.Notify(quit, os.Interrupt, syscall.SIGTERM)
	<-quit

	ctx, cancel := context.WithTimeout(context.Background(), 30*time.Second)
	defer cancel()

	if err := server.Shutdown(ctx); err != nil {
		log.Fatal().Stack().Err(err).Msg("")
	}

	log.Info().Msg("Shutting down server")
}
