# راهنمای عیب‌یابی اتصال به SQL Server

## مشکل فعلی
خطای اتصال به SQL Server در `192.168.8.100`:
```
A network-related or instance-specific error occurred while establishing a connection to SQL Server.
The server was not found or was not accessible.
```

## مراحل عیب‌یابی

### 1. بررسی دسترسی به SQL Server

از Docker container یا ماشین محلی، دسترسی به SQL Server را بررسی کنید:

```powershell
# تست اتصال از Docker container
docker exec -it web-api ping 192.168.8.100

# یا از PowerShell (اگر در ماشین محلی هستید)
Test-NetConnection -ComputerName 192.168.8.100 -Port 1433
```

### 2. بررسی اینکه SQL Server در حال اجرا است

در ماشین `192.168.8.100`:
```powershell
Get-Service -Name "*SQL*"
```

### 3. بررسی تنظیمات SQL Server

#### الف) فعال‌سازی TCP/IP
1. SQL Server Configuration Manager را باز کنید
2. SQL Server Network Configuration → Protocols for [Instance Name]
3. TCP/IP را Enable کنید
4. TCP/IP را راست‌کلیک → Properties
5. در تب IP Addresses:
   - IPAll → TCP Port = 1433
   - TCP Dynamic Ports را خالی کنید
6. SQL Server را Restart کنید

#### ب) فعال‌سازی SQL Server Authentication
1. SQL Server Management Studio (SSMS) را باز کنید
2. به Server راست‌کلیک → Properties
3. Security → SQL Server and Windows Authentication mode را انتخاب کنید
4. OK → SQL Server را Restart کنید

#### ج) فعال‌سازی Remote Connections
1. SQL Server Management Studio (SSMS) را باز کنید
2. به Server راست‌کلیک → Properties
3. Connections → Allow remote connections to this server را تیک بزنید
4. OK

### 4. بررسی Firewall

در ماشین `192.168.8.100`:
```powershell
# بررسی اینکه پورت 1433 باز است
Get-NetFirewallRule | Where-Object {$_.DisplayName -like "*SQL*"}

# اگر لازم است، پورت را باز کنید
New-NetFirewallRule -DisplayName "SQL Server" -Direction Inbound -Protocol TCP -LocalPort 1433 -Action Allow
```

### 5. تست اتصال با sqlcmd

```powershell
sqlcmd -S 192.168.8.100 -U sa -P "#mohsen@ma78" -Q "SELECT @@VERSION"
```

اگر این دستور کار کرد، SQL Server در دسترس است.

### 6. بررسی وجود دیتابیس Library

```powershell
sqlcmd -S 192.168.8.100 -U sa -P "#mohsen@ma78" -Q "SELECT name FROM sys.databases WHERE name = 'Library';"
```

### 7. ایجاد دیتابیس Library (اگر وجود ندارد)

```powershell
sqlcmd -S 192.168.8.100 -U sa -P "#mohsen@ma78" -Q "IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'Library') CREATE DATABASE Library;"
```

### 8. اجرای Migration

بعد از اینکه اتصال برقرار شد، migration را اجرا کنید:

```powershell
# از Docker container
docker exec -it web-api dotnet ef database update --project src/Infrastructure --startup-project src/Web.Api

# یا از ماشین محلی
cd C:\Users\MohsenArg\source\repos\VahuManah\VohuManah.Core
dotnet ef database update --project src/Infrastructure --startup-project src/Web.Api
```

### 9. استفاده از اسکریپت PowerShell

اسکریپت `scripts/check-and-migrate-database.ps1` را اجرا کنید:

```powershell
.\scripts\check-and-migrate-database.ps1
```

این اسکریپت:
- اتصال به SQL Server را بررسی می‌کند
- Migration ها را اجرا می‌کند
- وجود کاربر admin را بررسی می‌کند

## راه‌حل‌های جایگزین

### اگر SQL Server در ماشین محلی است (نه 192.168.8.100)

Connection String را در `appsettings.Development.json` تغییر دهید:

```json
{
  "ConnectionStrings": {
    "Database": "Server=localhost;Database=Library;User Id=sa;Password=#mohsen@ma78;TrustServerCertificate=True;Encrypt=False;MultipleActiveResultSets=true"
  }
}
```

### اگر از Docker استفاده می‌کنید و SQL Server در Host است

در `docker-compose.yml`:
```yaml
services:
  web-api:
    environment:
      - ConnectionStrings__Database=Server=host.docker.internal;Database=Library;User Id=sa;Password=#mohsen@ma78;TrustServerCertificate=True;Encrypt=False;MultipleActiveResultSets=true
    extra_hosts:
      - "host.docker.internal:host-gateway"
```

## بررسی لاگ‌های Migration

بعد از راه‌اندازی اپلیکیشن، لاگ‌ها را بررسی کنید:

```
[INF] Applying database migrations...
[INF] Database migrations applied successfully.
[INF] Admin user exists in database.
```

اگر این پیام‌ها را دیدید، همه چیز درست است.

## اطلاعات Login

بعد از اجرای موفق migration:
- **Username:** `admin`
- **Password:** `1234`

## اگر مشکل ادامه داشت

1. بررسی کنید که SQL Server Browser Service در حال اجرا است
2. بررسی کنید که SQL Server Agent در حال اجرا است (اگر لازم است)
3. بررسی کنید که Windows Firewall مانع اتصال نمی‌شود
4. بررسی کنید که Antivirus مانع اتصال نمی‌شود
5. از SQL Server Management Studio (SSMS) سعی کنید به `192.168.8.100` متصل شوید

