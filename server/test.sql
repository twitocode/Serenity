DROP TABLE IF EXISTS user_accounts;
DROP TABLE IF EXISTS users;

CREATE TABLE IF NOT EXISTS users (
  id INT PRIMARY KEY,
  email text,
  display_name text,
  password text
);

CREATE TABLE IF NOT EXISTS user_accounts (
  id INT PRIMARY KEY,
  email text,
  provider text,
  access_token text,
  user_id INT  FOREIGN KEY REFERENCES users(id)
);