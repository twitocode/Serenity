package account

import (
	"context"
	"database/sql"

	"github.com/pkg/errors"
	"github.com/rs/zerolog"

	"github.com/jmoiron/sqlx"
	"github.com/novaiiee/serenity/internal/domain"
)

type UserAccountRepository interface {
	CreateAccountWithInfo(ctx context.Context, info *domain.UserInfo, userId int) (int, error)
	GetUserAccount(ctx context.Context, provider, email string) (*domain.UserOauthInfo, error)
	GetAccountByEmail(ctx context.Context, provider, email string) (*domain.UserOauthInfo, error)
}

type userAccountRepository struct {
	db  *sqlx.DB
	log *zerolog.Logger
}

func NewUserAccountRepository(log *zerolog.Logger, db *sqlx.DB) UserAccountRepository {
	return &userAccountRepository{db: db, log: log}
}

func (r *userAccountRepository) CreateAccountWithInfo(ctx context.Context, info *domain.UserInfo, userId int) (int, error) {
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

func (r *userAccountRepository) GetUserAccount(ctx context.Context, provider, email string) (*domain.UserOauthInfo, error) {
	account := domain.UserOauthInfo{}
	err := r.db.GetContext(ctx, &account, `SELECT * FROM user_accounts WHERE email = $1 AND "provider" = $2`, email, provider)

	//Account exists
	if err != nil {
		return nil, errors.New("account exists but dont deal with it yet")
	}

	return &account, nil
}

func (r *userAccountRepository) GetAccountByEmail(ctx context.Context, provider, email string) (*domain.UserOauthInfo, error) {
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
