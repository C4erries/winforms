PROJECT=src/StationeryStore
DB_CONTAINER=stationery-postgres
DB_NAME=stationery
DB_USER=postgres

.PHONY: db seed run build down reset-db reset-db-seed logs psql clean

db:
	docker compose up -d --wait postgres

run: db
	dotnet run --project $(PROJECT)

build:
	dotnet build StationeryStore.slnx

down:
	docker compose down

reset-db:
	docker compose down -v
	docker compose up -d --wait postgres

seed: db
	docker exec -i -e PGPASSWORD=12345 $(DB_CONTAINER) psql -U $(DB_USER) -d $(DB_NAME) < db/seed.sql

reset-db-seed: reset-db seed

logs:
	docker compose logs -f postgres

psql:
	docker exec -it -e PGPASSWORD=12345 $(DB_CONTAINER) psql -U $(DB_USER) -d $(DB_NAME)

clean:
	dotnet clean StationeryStore.slnx
