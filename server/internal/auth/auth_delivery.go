package auth

import "net/http"

type Handler interface {
	ExternalProvider() http.HandlerFunc
	ExternalProviderCallback() http.HandlerFunc
  LoginWithEmailPassword() http.HandlerFunc
}
