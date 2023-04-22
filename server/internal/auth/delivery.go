package auth

import "net/http"

type AuthHandler interface {
	ExternalProvider() http.HandlerFunc
	ExternalProviderCallback() http.HandlerFunc
	LoginWithEmailPassword() http.HandlerFunc
	RegisterWithEmailPassword() http.HandlerFunc
}
