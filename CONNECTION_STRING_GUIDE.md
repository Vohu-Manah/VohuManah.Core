# راهنمای تنظیم Connection String برای SQL Server

## مشکلات احتمالی و راه‌حل‌ها:

### 1. اگر SQL Server Express نصب دارید:
```json
"Database": "Server=localhost\\SQLEXPRESS;Database=Library;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

### 2. اگر SQL Server با نام instance دیگر نصب دارید:
```json
"Database": "Server=localhost\\INSTANCE_NAME;Database=Library;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

### 3. اگر SQL Server با User/Password تنظیم شده:
```json
"Database": "Server=localhost;Database=Library;User Id=sa;Password=YourPassword;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

### 4. اگر از Docker استفاده می‌کنید:
```json
"Database": "Server=sql-server;Database=Library;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

### 5. اگر SQL Server روی پورت دیگری اجرا می‌شود:
```json
"Database": "Server=localhost,1433;Database=Library;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

## مراحل عیب‌یابی:

1. **بررسی اینکه SQL Server در حال اجرا است:**
   ```powershell
   Get-Service -Name "*SQL*"
   ```

2. **بررسی Instance Name:**
   ```powershell
   Get-Service | Where-Object {$_.Name -like "*SQL*"}
   ```

3. **بررسی با SQL Server Management Studio (SSMS):**
   - باز کردن SSMS
   - بررسی Server Name که در هنگام اتصال استفاده می‌کنید
   - معمولاً `localhost` یا `localhost\SQLEXPRESS` است

4. **بررسی Configuration Manager:**
   - باز کردن SQL Server Configuration Manager
   - بررسی که SQL Server (MSSQLSERVER) یا SQL Server (SQLEXPRESS) Running است
   - بررسی که TCP/IP Protocol Enabled است

5. **اگر SQL Server نصب نیست:**
   - نصب SQL Server Express (رایگان)
   - یا استفاده از Docker با docker-compose

## نکات مهم:

- `Integrated Security=True` برای Windows Authentication استفاده می‌شود
- `TrustServerCertificate=True` برای اتصال بدون SSL Certificate استفاده می‌شود (Development)
- `MultipleActiveResultSets=true` برای MARS (Multiple Active Result Sets) استفاده می‌شود

