package domain

type RegisterUserRequest struct {
	DisplayName string `json:"display_name" validate:"required,min=3"`
	Email       string `json:"email" validate:"required,email"`
	Password    string `json:"password" validate:"required,min=6"`
}

type LoginUserRequest struct {
	Identifier string `json:"identifier" validate:"required"`
	Password   string `json:"password" validate:"required"`
}
