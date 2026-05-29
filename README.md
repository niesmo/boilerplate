# BoilerplateApp

BoilerplateApp is a .NET Aspire solution that hosts a Blazor Server web app with ASP.NET Core Identity and PostgreSQL.

## High-Level Architecture

The solution is split into four projects:

- BoilerplateApp.AppHost: Aspire orchestration for local development.
- BoilerplateApp.Web: Blazor UI, Identity, EF Core data access, and app middleware.
- BoilerplateApp.ServiceDefaults: shared Aspire defaults for health checks, service discovery, resilience, and OpenTelemetry setup.
- BoilerplateApp.Web.Tests: unit and end-to-end tests.

At runtime, AppHost starts PostgreSQL and pgAdmin, creates the contact database, and launches the web app as the webfrontend resource.

## Tech Stack

- .NET 10
- Aspire Hosting 13.3.5
- Blazor Server (Razor Components)
- ASP.NET Core Identity
- EF Core + Npgsql
- xUnit

## Prerequisites

- .NET 10 SDK
- Docker Desktop or another local container runtime

## Getting Started

1. Restore and build:

```bash
dotnet restore
dotnet build BoilerplateApp.sln
```

1. Run the Aspire AppHost:

```bash
dotnet run --project BoilerplateApp.AppHost
```

1. Open the webfrontend URL shown in the Aspire output/dashboard.

## Authentication and Data Initialization

- The web app applies EF Core migrations on startup.
- Identity roles/users are seeded at startup from configuration.
- Local development bootstrap credentials are in BoilerplateApp.Web/appsettings.Development.json.

Default development admin values:

- Email: `admin@local.test`
- Password: ChangeThisAdminPwd!123
- Display Name: Local Admin

Change these before using this template outside local development.

## Running via Docker

### Docker (standalone)

Build the image from the solution root:

```bash
docker build -t boilerplate-web .
```

Run the container, supplying the PostgreSQL connection string as an environment variable:

```bash
docker run -p 8080:8080 \
  -e ConnectionStrings__contactdb="Host=<pg-host>;Port=5432;Database=contactdb;Username=<user>;Password=<pass>" \
  boilerplate-web
```

Open [http://localhost:8080](http://localhost:8080).

### Docker Compose (app + database together)

Start both services using the included `docker-compose.yml`:

```bash
docker compose up --build
```

Open [http://localhost:8080](http://localhost:8080).

> The web app runs EF Core migrations automatically on startup, so no separate migration step is needed.
> Replace the `changeme` password in `docker-compose.yml` before using outside local development.

## Testing

Run all tests:

```bash
dotnet test BoilerplateApp.sln
```

Run only the web test project:

```bash
dotnet test BoilerplateApp.Web.Tests/BoilerplateApp.Web.Tests.csproj
```

Current automated coverage includes:

- Message service unit tests.
- Aspire end-to-end smoke tests for the webfrontend home and sign-in routes.

## Common Commands

```bash
# Build everything
dotnet build BoilerplateApp.sln

# Run only the web app (without AppHost orchestration)
dotnet run --project BoilerplateApp.Web

# Run tests quickly after a prior build
dotnet test BoilerplateApp.Web.Tests/BoilerplateApp.Web.Tests.csproj --no-build
```

## Notes

- AppHost defines a health check on /health for the web app resource.
- Service defaults map /health and /alive endpoints in development.
