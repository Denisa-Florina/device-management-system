Write-Host "=== Device Management System - Setup ===" -ForegroundColor Cyan


$missing = @()
if (-not (Get-Command docker -ErrorAction SilentlyContinue)) { $missing += "Docker Desktop" }
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) { $missing += ".NET SDK" }
if (-not (Get-Command node -ErrorAction SilentlyContinue)) { $missing += "Node.js" }

if ($missing.Count -gt 0) {
    Write-Host "Missing prerequisites: $($missing -join ', ')" -ForegroundColor Red
    exit 1
}

$dockerInfo = docker info 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "Docker Desktop is not running. Please start it and try again." -ForegroundColor Red
    exit 1
}
Write-Host "[OK] Docker Desktop is running" -ForegroundColor Green

if (-not (Test-Path .env)) {
    Copy-Item .env.example .env
    (Get-Content .env) -replace 'YOUR_STRONG_PASSWORD_HERE', 'DevMgmt_Pass123!' | Set-Content .env
    Write-Host "[OK] Created .env file" -ForegroundColor Green
}

Write-Host "`nStarting Docker services..." -ForegroundColor Yellow
docker compose up -d
if ($LASTEXITCODE -ne 0) {
    Write-Host "Docker failed. Is Docker Desktop running?" -ForegroundColor Red
    exit 1
}
Write-Host "[OK] Docker services started" -ForegroundColor Green

Write-Host "`nWaiting for SQL Server..." -ForegroundColor Yellow
$ready = $false
for ($i = 0; $i -lt 30; $i++) {
    $result = docker exec device-management-system-db-1 /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "DevMgmt_Pass123!" -No -Q "SELECT 1" 2>$null
    if ($LASTEXITCODE -eq 0) { $ready = $true; break }
    Start-Sleep -Seconds 2
}
if (-not $ready) {
    Write-Host "SQL Server did not start in time." -ForegroundColor Red
    exit 1
}
Write-Host "[OK] SQL Server is ready" -ForegroundColor Green

$devSettings = "backend\DeviceManagement.API\appsettings.Development.json"
if (-not (Test-Path $devSettings)) {
    @'
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=DeviceManagement;User=sa;Password=DevMgmt_Pass123!;TrustServerCertificate=True"
  },
  "Jwt": {
    "SecretKey": "DevMgmt_JWT_SuperSecret_Key_2024_MustBe32Chars!"
  }
}
'@ | Set-Content $devSettings
    Write-Host "[OK] Created appsettings.Development.json" -ForegroundColor Green
}

Write-Host "`nApplying database migrations..." -ForegroundColor Yellow
Push-Location backend
dotnet ef database update --project DeviceManagement.Infrastructure --startup-project DeviceManagement.API
if ($LASTEXITCODE -ne 0) {
    Write-Host "Migration failed." -ForegroundColor Red
    Pop-Location
    exit 1
}
Pop-Location
Write-Host "[OK] Database migrations applied" -ForegroundColor Green

Write-Host "`nSeeding database..." -ForegroundColor Yellow
Get-Content database\02_seed_data.sql | docker exec -i device-management-system-db-1 /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "DevMgmt_Pass123!" -No
Write-Host "[OK] Database seeded" -ForegroundColor Green

Write-Host "`nInstalling frontend dependencies..." -ForegroundColor Yellow
Push-Location frontend\device-management-ui
npm install
Pop-Location
Write-Host "[OK] Frontend dependencies installed" -ForegroundColor Green

Write-Host "`nChecking Ollama AI model..." -ForegroundColor Yellow
$ollamaReady = $false
for ($i = 0; $i -lt 60; $i++) {
    $models = docker exec device-management-system-ollama-1 ollama list 2>$null
    if ($models -match "llama3.2") { $ollamaReady = $true; break }
    Start-Sleep -Seconds 5
}
if ($ollamaReady) {
    Write-Host "[OK] Ollama AI model ready" -ForegroundColor Green
} else {
    Write-Host "[WARN] Ollama model still downloading. AI features will work once complete." -ForegroundColor Yellow
}

Write-Host "`n=== Setup Complete ===" -ForegroundColor Cyan
Write-Host ""
Write-Host "To start the app:" -ForegroundColor White
Write-Host "  Backend:  cd backend && dotnet run --project DeviceManagement.API"
Write-Host "  Frontend: cd frontend\device-management-ui && ng serve"
Write-Host ""
Write-Host "  API:      http://localhost:5013"
Write-Host "  App:      http://localhost:4200"
Write-Host "  Login:    admin@email.com / Admin@123!"
Write-Host ""
