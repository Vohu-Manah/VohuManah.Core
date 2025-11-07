# راهنمای عیب‌یابی اتصال به SQL Server

## اگر نمی‌توانید به SQL Server متصل شوید:

### 1. بررسی Connection String:
```json
"Database": "Server=192.168.8.100;Database=Library;User Id=sa;Password=#mohsen@ma78;TrustServerCertificate=True;Encrypt=False;MultipleActiveResultSets=true"
```

### 2. بررسی اینکه SQL Server در حال اجرا است:
```powershell
sqlcmd -S 192.168.8.100 -U sa -P "#mohsen@ma78" -Q "SELECT @@VERSION"
```

### 3. بررسی اینکه دیتابیس Library وجود دارد:
```powershell
sqlcmd -S 192.168.8.100 -U sa -P "#mohsen@ma78" -Q "SELECT name FROM sys.databases WHERE name = 'Library';"
```

### 4. ایجاد دیتابیس Library (اگر وجود ندارد):
```powershell
sqlcmd -S 192.168.8.100 -U sa -P "#mohsen@ma78" -Q "IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'Library') CREATE DATABASE Library;"
```

### 5. اجرای Migration:
```powershell
cd VohuManah/src/Infrastructure
dotnet ef database update --startup-project ../Web.Api
```

### 6. بررسی مشکلات احتمالی:

#### مشکل 1: Firewall
- مطمئن شوید که پورت 1433 در SQL Server باز است
- در SQL Server Configuration Manager، TCP/IP را Enable کنید

#### مشکل 2: SQL Server Authentication
- مطمئن شوید که SQL Server Authentication Mode فعال است
- در SSMS: Right-click on Server → Properties → Security → SQL Server and Windows Authentication mode

#### مشکل 3: Remote Connections
- مطمئن شوید که SQL Server به remote connections اجازه می‌دهد
- در SQL Server Configuration Manager، TCP/IP Properties → IP Addresses → IPAll → TCP Port = 1433

#### مشکل 4: User Permissions
- مطمئن شوید که کاربر `sa` فعال است و password درست است
- اگر لازم است، کاربر جدید ایجاد کنید

### 7. تست Connection String:
```csharp
// در C# می‌توانید این را تست کنید:
var connectionString = "Server=192.168.8.100;Database=Library;User Id=sa;Password=#mohsen@ma78;TrustServerCertificate=True;Encrypt=False;MultipleActiveResultSets=true";
using var connection = new SqlConnection(connectionString);
connection.Open();
```

### 8. بررسی Logs:
- در Visual Studio، Output window را بررسی کنید
- در Console، خطاهای SQL Server را بررسی کنید

## فرمت‌های مختلف Connection String:

### Format 1 (Recommended):
```
Server=192.168.8.100;Database=Library;User Id=sa;Password=#mohsen@ma78;TrustServerCertificate=True;Encrypt=False;MultipleActiveResultSets=true
```

### Format 2:
```
Data Source=192.168.8.100;Initial Catalog=Library;User ID=sa;Password=#mohsen@ma78;TrustServerCertificate=True;Encrypt=False;MultipleActiveResultSets=true
```

### Format 3 (با Port):
```
Server=192.168.8.100,1433;Database=Library;User Id=sa;Password=#mohsen@ma78;TrustServerCertificate=True;Encrypt=False;MultipleActiveResultSets=true
```

## نکات مهم:

- `TrustServerCertificate=True` برای Development استفاده می‌شود
- `Encrypt=False` برای اتصال بدون SSL
- `MultipleActiveResultSets=true` برای Entity Framework Core
- Password باید با `#` و `@` درست escape شود

