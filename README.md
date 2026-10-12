# CampusEats — SE3090 Lab 11 (DevOps & Cloud Hosting)

Full-stack deployment of the CampusEats application built across Labs 02–04
and 08. This repository holds all three tiers and is the source every cloud
host deploys from.

**Student:** Chamika K.H — **IT number:** IT24102850

## Repository layout

| Path        | Tier        | Stack                        | Deployed to        |
|-------------|-------------|------------------------------|--------------------|
| `frontend/` | Frontend    | React 18 + Vite + TanStack Query | Vercel         |
| `backend/`  | Backend API | ASP.NET Core 8 + EF Core     | Render (Docker)    |
| —           | Database    | PostgreSQL 17                | Render PostgreSQL  |

## How the deployed tiers connect

```
Browser → Frontend (Vercel) → Backend API (Render) → PostgreSQL (Render)
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

### Backend (Render)

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

The deployed API serves the exact URL shapes the Lab 02 frontend already
called against json-server, so only the base URL changed when it moved to
the cloud.

| Method | Path                      | Auth   | Description                        |
|--------|---------------------------|--------|------------------------------------|
| GET    | `/api/menuItems`          | public | Search, filter, sort, page the menu |
| GET    | `/api/menuItems/{id}`     | public | One menu item                      |
| PATCH  | `/api/menuItems/{id}`     | public | Partial update (e.g. favourite)    |
| POST   | `/api/menuItems`          | JWT    | Add a menu item (staff)            |
| DELETE | `/api/menuItems/{id}`     | JWT    | Remove a menu item (staff)         |
| GET    | `/api/orders`             | public | Orders, optionally by `studentId`  |
| GET    | `/api/orders/{id}`        | public | One order                          |
| POST   | `/api/orders`             | public | Place an order                     |
| PATCH  | `/api/orders/{id}`        | public | Update status (e.g. `Cancelled`)   |
| POST   | `/api/auth/register`      | public | Create an account                  |
| POST   | `/api/auth/login`         | public | Obtain a JWT                       |
| GET    | `/health`                 | public | Status + database reachability     |
| GET    | `/swagger`                | public | Interactive API docs               |

`GET /api/menuItems` supports the json-server-style query string the
frontend sends: `?_page=1&_limit=6&_sort=name&_order=asc&q=kottu&category=Drinks`.
It returns the total row count in the `X-Total-Count` header, which the
CORS policy explicitly exposes so the browser lets the pager read it.
