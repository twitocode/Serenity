package main

import (
	"context"
	"flag"
	"fmt"
	"os"
	"os/signal"
	"syscall"
	"time"

	"github.com/charmbracelet/log"
	"github.com/novaiiee/serenity/config"
	"github.com/novaiiee/serenity/internal/server"
)

var docs = flag.String("docs", "", "Generates routing documentation for RESTful API - markdown, json, or raml.")

func main() {
	logger := log.NewWithOptions(os.Stderr, log.Options{
		ReportCaller:    true,
		ReportTimestamp: true,
	})
  
	flag.Parse()
	cfg, err := config.Get()

	if err != nil {
		log.Error(fmt.Sprintf("There was an error initializing the config: %v", err))
	}

	server := server.NewServer(logger, cfg)
	server.Run(docs)

	quit := make(chan os.Signal, 1)
	signal.Notify(quit, os.Interrupt, syscall.SIGTERM)
	<-quit

	ctx, cancel := context.WithTimeout(context.Background(), 30*time.Second)
	defer cancel()

	if err := server.Shutdown(ctx); err != nil {
		log.Fatal(err)
	}

	log.Info("Shutting down server")
}
