migrate-up:
	migrate -path db/migrations -database "postgresql://postgres:postgres@localhost:5432/serenity?sslmode=disable" up

migrate-down:
	migrate -path db/migrations -database "postgresql://postgres:postgres@localhost:5432/serenity?sslmode=disable" -verbose down

migrate-fix: 
	migrate -path db/migrations -database "postgresql://postgres:postgres@localhost:5432/serenity?sslmode=disable" -verbose force $(version)

new-migration:
	migrate create -ext sql -dir db/migrations -seq $(name)

server:
	go run cmd/main.go

server-docs:
	go run cmd/main.go -docs=json