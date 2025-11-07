# راهنمای تنظیم Connection String

## اگر خطای اتصال به SQL Server دریافت می‌کنید:

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
از docker-compose.yml استفاده کنید و Connection String را از Environment Variable تنظیم کنید.

## مراحل عیب‌یابی:

1. بررسی اینکه SQL Server در حال اجرا است
2. بررسی Instance Name در SQL Server Management Studio
3. بررسی Configuration Manager
4. اگر SQL Server نصب نیست، از Docker استفاده کنید

