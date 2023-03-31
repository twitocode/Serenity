package repository

import (
	"context"
	"database/sql"
	"errors"

	"github.com/jmoiron/sqlx"
	"github.com/novaiiee/serenity/internal/domain"
	"github.com/novaiiee/serenity/internal/user"
	"github.com/novaiiee/serenity/pkg/utils"
)

type userRepository struct {
	db *sqlx.DB
}

func NewUserRepository(db *sqlx.DB) user.UserRepository {
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

func (r *userRepository) GetUserAccount(ctx context.Context, provider, email string) (*domain.UserOauthInfo, error) {
	account := domain.UserOauthInfo{}
	err := r.db.GetContext(ctx, &account, `SELECT * FROM user_accounts WHERE email = $1 AND "provider" = $2`, email, provider)

	//Account exists
	if err != nil {
		return nil, errors.New("account exists but dont deal with it yet")
	}

	return &account, nil
}

func (r *userRepository) CreateUserWithInfo(ctx context.Context, info *domain.UserInfo) (int, error) {
	var id int
	query := `INSERT INTO users (email, display_name, "password", avatar_url, help_level) VALUES ($1, $2, $3, $4, $5) RETURNING user_id`
	rows, err := r.db.QueryContext(ctx, query, info.Email, info.DisplayName, utils.NewNullString(""), info.Avatar, 1)

	if err != nil {
		return 0, err
	}

	if rows.Next() {
		rows.Scan(&id)
	}

	return id, nil
}
func (r *userRepository) CreateAccountWithInfo(ctx context.Context, info *domain.UserInfo, userId int) (int, error) {
	var id int
	query := `INSERT INTO user_accounts (email, "provider", "access_token", user_id) VALUES ($1, $2, $3, $4) RETURNING user_account_id`
	rows, err := r.db.QueryContext(ctx, query, info.Email, info.Provider, info.AccessToken.AccessToken, userId)

	if err != nil {
		return 0, err
	}

	if rows.Next() {
		rows.Scan(&id)
	}

	return id, nil
}

func (r *userRepository) GetAccountByEmail(ctx context.Context, provider, email string) (*domain.UserOauthInfo, error) {
	account := domain.UserOauthInfo{}
	err := r.db.GetContext(ctx, &account, `SELECT * FROM user_accounts WHERE email = $1 AND "provider" = $2`, email, provider)

	if err == sql.ErrNoRows {
		return nil, nil
	}

	if err != nil {
		return nil, err
	}

	return &account, nil
}

func (r *userRepository) GetUserByEmail(ctx context.Context, email string) (*domain.User, error) {
	user := domain.User{}
	err := r.db.GetContext(ctx, &user, "SELECT user_id from users WHERE email = $1", email)

	if err == sql.ErrNoRows {
		return nil, nil
	}

	if err != nil {
		return nil, err
	}

	return &user, nil
}
