package user

import "net/http"

type Handler interface {
	GetUserById() http.HandlerFunc
}
