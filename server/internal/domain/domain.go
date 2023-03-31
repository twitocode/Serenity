package domain

import "database/sql"

type User struct {
	Id          int    `db:"user_id"`
	Email       string `db:"email"`
	DisplayName string `db:"display_name"`
	Password    sql.NullString `db:"password"`
	Avatar      string `db:"avatar_url"`
	HelpLevel   int    `db:"help_level"`
}

type UserOauthInfo struct {
	Id          int    `db:"user_account_id"`
	Provider    string `db:"provider"`
	Email       string `db:"email"`
	AccessToken string `db:"access_token"`
	UserId      int    `db:"user_id"`
}

type Post struct {
	Id      int    `db:"post_id"`
	Title   string `db:"title"`
	Content string `db:"content"`
	UserId  int    `db:"user_id"`
}

type Comment struct {
	Id     int `db:"comment_id"`
	PostId int `db:"post_id"`
	UserId int `db:"user_id"`
}
