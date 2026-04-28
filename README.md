# WinFormsApp3

WinForms lab project with PostgreSQL in Docker.

## Run

```powershell
make run
```

This starts PostgreSQL first, waits until it is ready, then runs the app.

## Visual Studio

Open `WinFormsApp3.sln` from the project root.

Form files are grouped by folders, but each form still has its `.cs`, `.Designer.cs` and `.resx` files together, so the WinForms designer should open normally.

Files like `.vs/` and `*.csproj.user` are local Visual Studio settings and are ignored by git.

## Useful Commands

```powershell
make db       # start PostgreSQL
make build    # build project
make run      # start PostgreSQL and run app
make down     # stop PostgreSQL
make reset-db # recreate database from db.sql
make psql     # open psql
```

Default database connection:

```text
Host=localhost;Port=5432;Username=postgres;Password=12345;Database=forma
```

## Structure

```text
src/WinFormsApp3/
  Program.cs
  Forms/
    Main/
    Clients/
    Products/
    Futura/
    Reports/
```
