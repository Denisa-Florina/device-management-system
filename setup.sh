#!/bin/bash
set -e

echo "=== Device Management System - Setup ==="

missing=""
command -v docker >/dev/null 2>&1 || missing="$missing Docker"
command -v dotnet >/dev/null 2>&1 || missing="$missing .NET_SDK"
command -v node >/dev/null 2>&1   || missing="$missing Node.js"

if [ -n "$missing" ]; then
    echo "Missing prerequisites:$missing"
    exit 1
fi

if ! docker info >/dev/null 2>&1; then
    echo "Docker is not running. Please start Docker Desktop and try again."
    exit 1
fi
echo "[OK] Docker is running"

if [ ! -f .env ]; then
    cp .env.example .env
    sed -i 's/YOUR_STRONG_PASSWORD_HERE/DevMgmt_Pass123!/' .env
    echo "[OK] Created .env file"
fi

echo ""
echo "Starting Docker services..."
docker compose up -d
echo "[OK] Docker services started"

echo ""
echo "Waiting for SQL Server..."
for i in $(seq 1 30); do
    if docker exec device-management-system-db-1 /opt/mssql-tools18/bin/sqlcmd \
        -S localhost -U sa -P "DevMgmt_Pass123!" -No -Q "SELECT 1" >/dev/null 2>&1; then
        echo "[OK] SQL Server is ready"
        break
    fi
    if [ "$i" -eq 30 ]; then
        echo "SQL Server did not start in time."
        exit 1
    fi
    sleep 2
done

DEV_SETTINGS="backend/DeviceManagement.API/appsettings.Development.json"
if [ ! -f "$DEV_SETTINGS" ]; then
    cat > "$DEV_SETTINGS" << 'EOF'
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=DeviceManagement;User=sa;Password=DevMgmt_Pass123!;TrustServerCertificate=True"
  },
  "Jwt": {
    "SecretKey": "DevMgmt_JWT_SuperSecret_Key_2024_MustBe32Chars!"
  }
}
EOF
    echo "[OK] Created appsettings.Development.json"
fi

echo ""
echo "Applying database migrations..."
cd backend
dotnet ef database update --project DeviceManagement.Infrastructure --startup-project DeviceManagement.API
cd ..
echo "[OK] Database migrations applied"

echo ""
echo "Seeding database..."
docker exec -i device-management-system-db-1 /opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U sa -P "DevMgmt_Pass123!" -No < database/02_seed_data.sql
echo "[OK] Database seeded"

echo ""
echo "Installing frontend dependencies..."
cd frontend/device-management-ui
npm install
cd ../..
echo "[OK] Frontend dependencies installed"

echo ""
echo "Checking Ollama AI model..."
for i in $(seq 1 60); do
    if docker exec device-management-system-ollama-1 ollama list 2>/dev/null | grep -q "llama3.2"; then
        echo "[OK] Ollama AI model ready"
        break
    fi
    if [ "$i" -eq 60 ]; then
        echo "[WARN] Ollama model still downloading. AI features will work once complete."
    fi
    sleep 5
done

echo ""
echo "=== Setup Complete ==="
echo ""
echo "To start the app:"
echo "  Backend:  cd backend && dotnet run --project DeviceManagement.API"
echo "  Frontend: cd frontend/device-management-ui && ng serve"
echo ""
echo "  API:      http://localhost:5013"
echo "  App:      http://localhost:4200"
echo "  Login:    admin@email.com / Admin@123!"
echo ""
