# WinFormsApp3

WinForms lab project with PostgreSQL in Docker.

## Database

Start PostgreSQL:

```powershell
docker compose up -d
```

The first start creates database `forma` and applies `db.sql` automatically.

Connection settings used by the app by default:

```text
Host=localhost;Port=5432;Username=postgres;Password=PAROL12345;Database=forma
```

To recreate the database from `db.sql`:

```powershell
docker compose down -v
docker compose up -d
```

## Run

```powershell
dotnet run
```
