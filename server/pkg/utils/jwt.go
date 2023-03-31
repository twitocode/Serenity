package utils

import (
	"github.com/golang-jwt/jwt/v5"
)

func GenerateJwt(k string, claims jwt.MapClaims) (string, error) {
	t := jwt.NewWithClaims(jwt.SigningMethodHS256, claims)
	s, err := t.SignedString([]byte(k))

	if err != nil {
		return "", err
	}

	return s, nil
}
