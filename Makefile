PROJECT=src/WinFormsApp3

.PHONY: db run build down reset-db logs psql clean

db:
	docker compose up -d --wait postgres

run: db
	dotnet run --project $(PROJECT)

build:
	dotnet build WinFormsApp3.sln

down:
	docker compose down

reset-db:
	docker compose down -v
	docker compose up -d --wait postgres

logs:
	docker compose logs -f postgres

psql:
	docker exec -it -e PGPASSWORD=12345 winforms-postgres psql -U postgres -d forma

clean:
	dotnet clean WinFormsApp3.sln
