package user

import "net/http"

type UserHandler interface {
	GetUserById() http.HandlerFunc
}
