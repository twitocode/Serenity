package user

import (
	"github.com/jmoiron/sqlx"
)

type UserRepository interface {
	GetUserById(id int) (*User, error)
}

type userRepository struct {
	db *sqlx.DB
}

func NewUserRepository(db *sqlx.DB) UserRepository {
	return &userRepository{db}
}

func (r *userRepository) GetUserById(id int) (*User, error) {
	user := User{}
	err := r.db.Get(&user, "SELECT * from users WHERE id = $1", id)

	if err != nil {
		return nil, err
	}

	return &user, nil
}
