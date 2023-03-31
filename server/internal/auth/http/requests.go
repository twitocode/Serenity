package http

import "github.com/go-playground/validator/v10"

type RegisterUserRequest struct {
	DisplayName string `json:"display_name" validate:"required,min=3"`
	Email       string `json:"email" validate:"required,email"`
	Password    string `json:"password" validate:"required,min=6"`
}

func (r RegisterUserRequest) Validate() validator.ValidationErrors {
	validate := validator.New()
	if err := validate.Struct(r); err != nil {
		return err.(validator.ValidationErrors)
	}

	return nil
}
