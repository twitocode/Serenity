CREATE TABLE IF NOT EXISTS users (
  id INT IDENTITY(1, 1) PRIMARY KEY,
  email text,
  display_name text,
  password text
);

CREATE TABLE IF NOT EXISTS user_accounts (
  id INT PRIMARY KEY,
  email text,
  provider text,
  access_token text,
  user_id INT UNIQUE,
  FOREIGN KEY (user_id) REFERENCES users(id)
);