# Device Management System

A full-stack device management application built with **.NET 10** and **Angular 21**. Manage mobile devices, assign them to users, and generate AI-powered device descriptions using a local LLM.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/) (includes npm)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

## Getting Started

There are two ways to set up the project:

### Option 1 -- Manual Setup

#### 1. Clone and configure environment

```bash
git clone <repository-url>
cd device-management-system
cp .env.example .env
```

Edit `.env` and set a strong SA password:

```
SA_PASSWORD=DevMgmt_Pass123!
DB_NAME=DeviceManagement
```

#### 2. Start Docker services

This starts SQL Server and Ollama (local AI model):

```bash
docker compose up -d
```

Wait ~1 minute for the `ollama-pull` service to download the AI model (~2GB, one-time only). You can check progress with:

```bash
docker compose logs ollama-pull -f
```

#### 3. Configure the backend

Update the connection string in `backend/DeviceManagement.API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=DeviceManagement;User=sa;Password=DevMgmt_Pass123!;TrustServerCertificate=True"
  },
  "Jwt": {
    "SecretKey": "YourSecretKey_MustBeAtLeast32Characters!!"
  }
}
```

#### 4. Apply database migrations

```bash
cd backend
dotnet ef database update --project DeviceManagement.Infrastructure --startup-project DeviceManagement.API
```

#### 5. Seed the database (optional)

Run the SQL seed scripts against the Docker SQL Server container.

**PowerShell:**
```powershell
Get-Content database\01_create_database.sql | docker exec -i device-management-system-db-1 /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "DevMgmt_Pass123!" -No
Get-Content database\02_seed_data.sql | docker exec -i device-management-system-db-1 /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "DevMgmt_Pass123!" -No
```

**Bash / macOS / Linux:**
```bash
docker exec -i device-management-system-db-1 /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "DevMgmt_Pass123!" -No < database/01_create_database.sql
docker exec -i device-management-system-db-1 /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "DevMgmt_Pass123!" -No < database/02_seed_data.sql
```

#### 6. Run the backend

```bash
cd backend
dotnet run --project DeviceManagement.API
```

The API starts at **http://localhost:5013**. API docs are available at `/scalar/v1`.

#### 7. Run the frontend

```bash
cd frontend/device-management-ui
npm install
ng serve
```

The app starts at **http://localhost:4200**.

---

### Option 2 -- Quick Setup (one command)

A setup script that handles everything automatically: Docker services, database migrations, seeding, frontend dependencies, and AI model download.

**Windows (PowerShell):**
```powershell
git clone <repository-url>
cd device-management-system
.\setup.ps1
```

**macOS / Linux:**
```bash
git clone <repository-url>
cd device-management-system
chmod +x setup.sh && ./setup.sh
```

Once complete, start the app:
```bash
# Terminal 1 - Backend
cd backend && dotnet run --project DeviceManagement.API

# Terminal 2 - Frontend
cd frontend/device-management-ui && ng serve
```

## Default Credentials

| Role     | Email              | Password     |
|----------|--------------------|--------------|
| Admin    | admin@email.com    | Admin@123!   |

New users can register via the app and receive the "Customer" role.

## AI Description Generator

The app includes an AI-powered device description generator using **Ollama** (a free, local LLM). When creating or editing a device, click the **"AI Generate"** button to automatically generate a description based on the device specs.

**How it works:**
- The backend sends device specs to Ollama's local API
- Ollama runs the `llama3.2` model locally (no API key, no cloud, no cost)
- Returns a concise, human-readable description

To change the model, edit `Ollama:Model` in `appsettings.json`.

## Running Tests

```bash
cd backend
dotnet test DeviceManagement.IntegrationTests
```

Integration tests use an in-memory database and a fake AI service, so they run without Docker.

## Project Structure

```
device-management-system/
├── backend/
│   ├── DeviceManagement.API/              # REST API, controllers
│   ├── DeviceManagement.Application/      # Services, DTOs, interfaces
│   ├── DeviceManagement.Domain/           # Entities, enums
│   ├── DeviceManagement.Infrastructure/   # EF Core, repositories, Identity
│   └── DeviceManagement.IntegrationTests/ # Integration tests
├── frontend/
│   └── device-management-ui/              # Angular 21 app
├── database/
│   ├── 01_create_database.sql             # DB schema
│   └── 02_seed_data.sql                   # Sample data (25 devices, 5 users)
└── docker-compose.yml                     # SQL Server + Ollama
```