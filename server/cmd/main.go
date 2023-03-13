package main

import (
	"context"
	"fmt"
	"os"
	"os/signal"
	"syscall"
	"time"

	"github.com/charmbracelet/log"
	"github.com/novaiiee/serenity/config"
	"github.com/novaiiee/serenity/pkg/server"
)

func main() {
	cfg, err := config.Get()

	if err != nil {
		log.Error(fmt.Sprintf("There was an error initializing the config: %v", err))
	}

	server := server.NewServer(cfg)
	server.Run()

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
