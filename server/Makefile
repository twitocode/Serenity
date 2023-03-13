migrateup:
	migrate -path database/migrations -database "postgresql://postgres:postgres@localhost:5432/serenity?sslmode=disable" -verbose up

migratedown:
	migrate -path database/migrations -database "postgresql://postgres:postgres@localhost:5432/serenity?sslmode=disable" -verbose down

migratefix: 
	migrate -path database/migrations/ -database "postgresql://postgres:postgres@localhost:5432/serenity?sslmode=disable" force 1