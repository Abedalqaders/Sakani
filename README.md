# Sakani
🏗️ Real Estate Management SaaS - Backend Setup Guide
This documentation provides the necessary steps to set up the development environment, launch the database via Docker, and synchronize the schema using EF Core.

1. Prerequisites
Ensure the following tools are installed on your machine:

Docker Desktop: Download here

.NET 8 SDK: Download here

DataGrip (or any Database Client): Download here

2. Infrastructure Setup (Docker)
We use Docker to ensure consistent database environments across the team.

Open your terminal in the project root directory (where docker-compose.yml is located).

Run the following command:

Bash
docker-compose up -d
Verify the container is running:

Bash
docker ps
You should see realestate_postgres with status Up.

3. Database Migration
After the database is running, you must apply the schema migrations to create the tables.

Open the solution in Visual Studio.

Open Package Manager Console (Tools > NuGet Package Manager).

Set Default Project to Infrastructure.

Set Startup Project in Solution Explorer to API.

Execute the following command:

Bash
Update-Database
4. Connecting DataGrip to Docker
To manage and view the data, follow these steps in DataGrip:

Click the + icon -> Data Source -> PostgreSQL.

Fill in the connection details:

Host: localhost

Port: 5432

User: postgres

Password: (Refer to docker-compose.yml)

Database: RealEstateDb

Drivers: If prompted, click the Download link at the bottom of the window to install the PostgreSQL drivers.

Click Test Connection. Once the green checkmark appears, click OK.

Schema View: If tables are hidden, right-click the connection -> Properties -> Schemas -> Check All databases.
