# Script to check SQL Server connection and apply migrations
# Usage: .\scripts\check-and-migrate-database.ps1

$ErrorActionPreference = "Stop"

Write-Host "Checking SQL Server connection..." -ForegroundColor Cyan

$connectionString = "Server=192.168.8.100;Database=Library;User Id=sa;Password=#mohsen@ma78;TrustServerCertificate=True;Encrypt=False;MultipleActiveResultSets=true"

# Test connection
try {
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    Write-Host "✓ SQL Server connection successful!" -ForegroundColor Green
    $connection.Close()
}
catch {
    Write-Host "✗ Failed to connect to SQL Server: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please check:" -ForegroundColor Yellow
    Write-Host "  1. SQL Server is running on 192.168.8.100" -ForegroundColor Yellow
    Write-Host "  2. SQL Server allows remote connections" -ForegroundColor Yellow
    Write-Host "  3. Firewall allows connection on port 1433" -ForegroundColor Yellow
    Write-Host "  4. SQL Server Authentication is enabled" -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "Applying database migrations..." -ForegroundColor Cyan

# Apply migrations
try {
    Push-Location $PSScriptRoot\..
    dotnet ef database update --project src/Infrastructure --startup-project src/Web.Api
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Migrations applied successfully!" -ForegroundColor Green
    }
    else {
        Write-Host "✗ Failed to apply migrations" -ForegroundColor Red
        exit 1
    }
    Pop-Location
}
catch {
    Write-Host "✗ Error applying migrations: $_" -ForegroundColor Red
    Pop-Location
    exit 1
}

Write-Host ""
Write-Host "Checking if admin user exists..." -ForegroundColor Cyan

# Check if admin user exists
try {
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    
    $query = "SELECT COUNT(*) FROM [dbo].[LibraryUsers] WHERE [UserName] = 'admin'"
    $command = New-Object System.Data.SqlClient.SqlCommand($query, $connection)
    $count = $command.ExecuteScalar()
    
    if ($count -gt 0) {
        Write-Host "✓ Admin user exists!" -ForegroundColor Green
        Write-Host ""
        Write-Host "You can login with:" -ForegroundColor Cyan
        Write-Host "  Username: admin" -ForegroundColor White
        Write-Host "  Password: 1234" -ForegroundColor White
    }
    else {
        Write-Host "✗ Admin user does not exist!" -ForegroundColor Red
        Write-Host "  Please check the migration EnsureAdminUserExists" -ForegroundColor Yellow
    }
    
    $connection.Close()
}
catch {
    Write-Host "✗ Error checking admin user: $_" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Done!" -ForegroundColor Green

