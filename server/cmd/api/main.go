package main

import (
	"context"
	"flag"
	"fmt"
	"os"
	"os/signal"
	"syscall"
	"time"

	"github.com/novaiiee/serenity/config"
	"github.com/novaiiee/serenity/internal/server"
	"github.com/rs/zerolog"
	"github.com/rs/zerolog/log"
	"github.com/rs/zerolog/pkgerrors"
)

var docs = flag.String("docs", "", "Generates routing documentation for RESTful API - markdown, json, or raml.")

func main() {
	// log.Logger = log.Output(zerolog.ConsoleWriter{Out: os.Stderr,}).With().
	zerolog.TimeFieldFormat = zerolog.TimeFormatUnix
	zerolog.ErrorStackMarshaler = pkgerrors.MarshalStack
	// log.Error().Stack().Err(errors.New("You suck balls")).Msg("")

	flag.Parse()
	cfg, err := config.Get()

	if err != nil {
		log.Error().Stack().Msg(fmt.Sprintf("There was an error initializing the config: %v", err))
	}

	server := server.NewServer(cfg)
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
