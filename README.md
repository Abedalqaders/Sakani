# 🏢 Real Estate SaaS - Backend 

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512bd4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-336791?logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![Docker](https://img.shields.io/badge/Docker-Container-2496ed?logo=docker&logoColor=white)](https://www.docker.com/)

A robust multi-tenant SaaS platform designed for professional real estate management. This backend handles data isolation, automated migrations, and complex contract logic.

## 🚀 Quick Start Guide

Follow these steps to synchronize the development environment on your local machine.

### 1. Prerequisites 📋
Ensure the following are installed:
* **Docker Desktop** (To run the database)
* **.NET 8 SDK** (To run the API)
* **DataGrip** or **pgAdmin** (To manage data)

### 2. Infrastructure Setup (Docker) 🐳
We use Docker to spin up the PostgreSQL instance instantly without manual installation.

1. Open your terminal in the project root.
2. Run the command:
   docker-compose up -d
3. **Verify:** Run `docker ps`. You should see the `realestate_postgres` container running on port `5432`.

## 🛠️ Database Configuration

### Entity Framework Migrations
The schema is managed via EF Core. You must sync your local database after the Docker container is active.

1. Open the solution in **Visual Studio**.
2. Open **Package Manager Console**.
3. Set **Default Project** to `Infrastructure`.
4. Set **Startup Project** to `API`.
5. Run the following command:
   Update-Database

## 🗄️ Database Management (DataGrip)

Connect **DataGrip** to the Dockerized database using these settings:

| Property | Value |
| :--- | :--- |
| **Driver** | PostgreSQL |
| **Host** | `localhost` |
| **Port** | `5432` |
| **User** | `postgres` |
| **Password** | *Refer to docker-compose.yml* |
| **Database** | `RealEstateDb` |

> [!TIP]
> If tables are missing after connecting: Right-click Connection -> Properties -> Schemas -> Check All databases or public.

## 🏗️ Project Architecture
The project follows **N-Tier/Clean Architecture** principles:

* **Domain:** Core Entities and Business Enums.
* **Application:** Business logic, DTOs, and Interfaces.
* **Infrastructure:** PostgreSQL Implementation, DbContext, and Migrations.
* **API:** REST Endpoints, Middlewares, and Authentication.

## ⚠️ Troubleshooting

* **Port 5432 Conflict:** If you have a local PostgreSQL service running, stop it or change the host port in `docker-compose.yml`.
* **Docker Daemon:** Ensure Docker Desktop is fully loaded (Green icon) before running commands.
* **Update-Database Failure:** Verify the ConnectionStrings in appsettings.json matches the credentials in your Docker file.connecting: Right-click Connection -> Properties -> Schemas -> Check All databases or public.🏗️ Project ArchitectureThe project follows N-Tier/Clean Architecture principles:Domain: Core Entities and Business Enums.Application: Business logic, DTOs, and Interfaces.Infrastructure: PostgreSQL Implementation, DbContext, and Migrations.API: REST Endpoints, Middlewares, and Authentication.⚠️ TroubleshootingPort 5432 Conflict: If you have a local PostgreSQL service running, stop it or change the host port in docker-compose.yml.Docker Daemon: Ensure Docker Desktop is fully loaded (Green icon) before running commands.Update-Database Failure: Verify the ConnectionStrings in appsettings.json matches the credentials in your Docker file.
