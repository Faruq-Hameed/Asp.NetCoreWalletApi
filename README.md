# WalletApi — a minimal but complete .NET 10 backend

A learning project covering: EF Core + PostgreSQL, JWT auth, password hashing,
a joined query (Wallet → Transactions), DTOs, DI, global exception middleware.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) — `dotnet --version` should print `10.x`
- PostgreSQL running locally (easiest via Docker):
  ```bash
  docker run --name walletdb -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=walletdb -p 5432:5432 -d postgres
  ```
- The EF Core CLI tool (one-time, global install):
  ```bash
  dotnet tool install --global dotnet-ef
  ```

## Setup

```bash
cd WalletApi
dotnet restore
```

Before running, replace `Jwt:Key` in `appsettings.json` with your own long random
string (32+ characters) — never ship the placeholder value.

## Create the database schema

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

This generates a `Migrations/` folder (EF Core's equivalent of TypeORM migration
files) and applies it to your Postgres instance.

## Run it

```bash
dotnet run
```

Then open the printed URL + `/swagger` (e.g. `https://localhost:5001/swagger`) —
Swagger UI lets you call every endpoint from the browser, including authorized ones
(click "Authorize" and paste `Bearer <your-token>` after logging in).

## Try the flow

1. `POST /api/auth/register` with `{ "username": "faruq", "password": "password123" }`
2. `POST /api/auth/login` with the same credentials → copy the returned `token`
3. Authorize in Swagger with `Bearer <token>`
4. `POST /api/wallet/transactions` with `{ "amount": 500, "type": "Credit", "description": "Test deposit" }`
5. `GET /api/wallet/me` → see your balance and the joined transaction list

## What to poke at next

- Add a `[Authorize(Roles = "Admin")]` endpoint and a `Role` field on `User` to see role-based auth
- Add FluentValidation instead of Data Annotations for more complex validation rules
- Add a repository layer between controllers and `AppDbContext` if you want to practice that pattern
- Swap `Include()` for `.Select()` projections to see the difference in generated SQL (check with `dotnet ef` logging or a tool like pgAdmin)
