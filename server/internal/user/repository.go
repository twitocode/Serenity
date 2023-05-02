package user

import (
	"context"
	"database/sql"

	"github.com/jmoiron/sqlx"
	"github.com/novaiiee/serenity/internal/domain"
	"github.com/novaiiee/serenity/pkg/database"
)

type UserRepository interface {
	CreateUserWithInfo(ctx context.Context, info *domain.UserInfo) (int, error)
	GetUserById(ctx context.Context, id int) (*domain.User, error)
	GetUserIdByEmail(ctx context.Context, email string) (int, error)
	GetUserByEmail(ctx context.Context, email string) (*domain.User, error)
	GetUserIdByDisplayName(ctx context.Context, displayName string) (int, error)
	GetUserPasswordByIdentifier(ctx context.Context, identifier string) (*domain.User, error)
}

type userRepository struct {
	db *sqlx.DB
}

func NewUserRepository(db *sqlx.DB) UserRepository {
	return &userRepository{db}
}

func (r *userRepository) GetUserById(ctx context.Context, id int) (*domain.User, error) {
	user := domain.User{}
	err := r.db.GetContext(ctx, &user, "SELECT * from users WHERE user_id = $1", id)
	if err == sql.ErrNoRows {
		return nil, nil
	}

	if err != nil {
		return nil, err
	}

	return &user, nil
}

func (r *userRepository) GetUserIdByEmail(ctx context.Context, email string) (int, error) {
	user := domain.User{}
	err := r.db.GetContext(ctx, &user, "SELECT user_id from users WHERE email = $1", email)

	if err == sql.ErrNoRows {
		return 0, nil
	}

	if err != nil {
		return 0, err
	}

	return user.Id, nil
}

func (r *userRepository) CreateUserWithInfo(ctx context.Context, info *domain.UserInfo) (int, error) {
	var id int
	query := `INSERT INTO users (email, display_name, "password", avatar_url, help_level) VALUES ($1, $2, $3, $4, $5) RETURNING user_id`
	rows, err := r.db.QueryContext(ctx, query, info.Email, info.DisplayName, database.NewNullString(info.Password), info.Avatar, 1)

	if err != nil {
		return 0, err
	}

	if rows.Next() {
		rows.Scan(&id)
	}

	return id, nil
}

func (r *userRepository) GetUserByEmail(ctx context.Context, email string) (*domain.User, error) {
	user := domain.User{}
	err := r.db.GetContext(ctx, &user, "SELECT * from users WHERE email = $1", email)

	if err == sql.ErrNoRows {
		return nil, nil
	}

	if err != nil {
		return nil, err
	}

	return &user, nil
}

func (r *userRepository) GetUserIdByDisplayName(ctx context.Context, displayName string) (int, error) {
	user := domain.User{}
	err := r.db.GetContext(ctx, &user, "SELECT user_id from users WHERE display_name = $1", displayName)

	if err == sql.ErrNoRows {
		return 0, nil
	}

	if err != nil {
		return 0, err
	}

	return user.Id, nil
}

func (r *userRepository) GetUserPasswordByIdentifier(ctx context.Context, identifier string) (*domain.User, error) {
	user := domain.User{}
	err := r.db.GetContext(ctx, &user, "SELECT user_id, password from users WHERE display_name = $1 OR email = $1", identifier)

	if err == sql.ErrNoRows {
		return nil, nil
	}

	if err != nil {
		return nil, err
	}

	return &user, nil
}
