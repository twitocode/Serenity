package user


type User struct {
  Email string `db:"email"`
  DisplayName string `db:"display_name"`
  Password string `db:"password"`
  Id string `db:"id"`
}

type UserAccount struct {
  Id int `db:"id"`
  Provider string `db:"provider"`
  Email string `db:"Email"`
  AccessToken string `db:"access_token"`
  UserId string `db:"user_id"`
}
