CREATE TABLE users (
  user_id SERIAL PRIMARY KEY NOT NULL, 
  email TEXT NOT NULL, 
  display_name TEXT NOT NULL, 
  "password" TEXT, 
  avatar_url TEXT NOT NULL, 
  help_level INT NOT NULL
);

CREATE TABLE user_accounts (
  user_account_id SERIAL PRIMARY KEY NOT NULL,
  email TEXT NOT NULL,
  "provider" TEXT NOT NULL,
  access_token TEXT NOT NULL,
  user_id SERIAL NOT NULL,
  FOREIGN KEY (user_id) REFERENCES users(user_id)
);

CREATE TABLE posts (
  post_id SERIAL PRIMARY KEY NOT NULL,
  title TEXT NOT NULL,
  content TEXT,

  user_id SERIAL NOT NULL,
  FOREIGN KEY (user_id) REFERENCES users(user_id)
);

CREATE TABLE comments (
  comment_id SERIAL PRIMARY KEY NOT NULL,
  user_id SERIAL NOT NULL, 
  post_id SERIAL NOT NULL,

  FOREIGN KEY (post_id) REFERENCES posts(post_id),
  FOREIGN KEY (user_id) REFERENCES users(user_id)
);