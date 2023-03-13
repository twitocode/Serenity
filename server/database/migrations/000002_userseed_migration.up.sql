CREATE SEQUENCE id_seq
start 1
increment 1;

INSERT INTO users (id, email, display_name, password)
VALUES (nextval('id_seq'), 'toheebeji@gmail.com', 'Twito', 'twilight123');