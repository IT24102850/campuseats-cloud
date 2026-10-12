# CampusEats — SE3090 Lab 11 (DevOps & Cloud Hosting)

Full-stack deployment of the CampusEats application built across Labs 02–04
and 08. This repository holds all three tiers and is the source every cloud
host deploys from.

**Student:** Chamika K.H — **IT number:** IT24102850

## Repository layout

| Path        | Tier        | Stack                        | Deployed to        |
|-------------|-------------|------------------------------|--------------------|
| `frontend/` | Frontend    | React 19 + Vite              | Vercel             |
| `backend/`  | Backend API | ASP.NET Core 8 + EF Core     | Railway (Docker)   |
| —           | Database    | PostgreSQL 17                | Railway PostgreSQL |

## How the deployed tiers connect

```
Browser → Frontend (Vercel) → Backend API (Railway) → PostgreSQL (Railway)
                VITE_API_URL          ConnectionStrings__Default
```

Each arrow is configuration, not code: the frontend learns the API's address
from `VITE_API_URL`, and the API learns the database's address from
`ConnectionStrings__Default`. Both are set on the hosting platform.

## Environment variables

Nothing secret is committed. `appsettings.json` ships with empty placeholders
and the real values live only in the platform dashboards.

### Frontend (Vercel)

| Variable       | Purpose                             |
|----------------|-------------------------------------|
| `VITE_API_URL` | Base URL of the deployed backend API |

Vite inlines `VITE_*` variables **at build time**, so the frontend must be
redeployed after this value changes.

### Backend (Railway)

| Variable                     | Purpose                                      |
|------------------------------|----------------------------------------------|
| `ConnectionStrings__Default` | PostgreSQL connection string (SSL required)  |
| `Jwt__Key`                   | Production token-signing secret              |
| `Jwt__Issuer`                | `CampusEats`                                 |
| `Jwt__Audience`              | `CampusEatsUsers`                            |
| `FRONTEND_ORIGIN`            | Exact frontend origin allowed through CORS   |
| `PORT`                       | Injected by the platform; the app binds it   |

The double underscore `__` maps to nested .NET configuration, so
`ConnectionStrings__Default` overrides `ConnectionStrings:Default`.

## Running locally

```bash
# frontend
cd frontend && npm install && npm run build && npm run preview

# backend (needs a PostgreSQL connection string in the environment)
cd backend && dotnet run
```

## API endpoints

| Method | Path             | Auth      | Description              |
|--------|------------------|-----------|--------------------------|
| GET    | `/api/menu`      | public    | List menu items          |
| GET    | `/api/menu/{id}` | public    | One menu item            |
| POST   | `/api/menu`      | JWT       | Create a menu item       |
| PUT    | `/api/menu/{id}` | JWT       | Update a menu item       |
| DELETE | `/api/menu/{id}` | JWT       | Delete a menu item       |
| POST   | `/api/auth/register` | public | Create an account      |
| POST   | `/api/auth/login`    | public | Obtain a JWT           |
| GET    | `/health`        | public    | Status + DB reachability |
| GET    | `/swagger`       | public    | Interactive API docs     |
