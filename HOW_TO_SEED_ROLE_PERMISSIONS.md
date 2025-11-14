# راهنمای اجرای Script برای Seed کردن Role Permissions

این راهنما نحوه اجرای script SQL برای اضافه کردن همه endpointهای Library به نقش Admin را توضیح می‌دهد.

## فایل Script

فایل `seed-role-permissions.sql` شامل تمام endpointهای موجود در پوشه Library است.

## نحوه اجرا

### روش 1: استفاده از SQL Server Management Studio (SSMS)

1. فایل `seed-role-permissions.sql` را باز کنید
2. به دیتابیس خود متصل شوید
3. Script را اجرا کنید (F5 یا Execute)

### روش 2: استفاده از Command Line

```bash
sqlcmd -S server_name -d database_name -i seed-role-permissions.sql
```

### روش 3: استفاده از Entity Framework Migration

اگر می‌خواهید این کار را از طریق EF Core انجام دهید، می‌توانید یک migration ایجاد کنید:

```bash
dotnet ef migrations add SeedRolePermissions --project src/Infrastructure --startup-project src/Web.Api
```

سپس در فایل migration ایجاد شده، کد SQL را اضافه کنید.

## بررسی نتیجه

بعد از اجرای script، می‌توانید با این query بررسی کنید:

```sql
SELECT 
    r.Name AS RoleName,
    rp.EndpointName,
    COUNT(*) OVER (PARTITION BY r.Name) AS TotalEndpoints
FROM [dbo].[RolePermission] rp
INNER JOIN [dbo].[Role] r ON rp.RoleId = r.Id
WHERE r.Name = 'Admin'
ORDER BY rp.EndpointName;
```

## نکات مهم

1. **این script از MERGE استفاده می‌کند** - بنابراین می‌توانید آن را چند بار اجرا کنید بدون اینکه duplicate ایجاد شود
2. **نقش Admin باید از قبل وجود داشته باشد** - اگر وجود نداشت، script خطا می‌دهد
3. **جدول RolePermission باید از قبل ایجاد شده باشد** - اگر migration برای RolePermission اجرا نشده، ابتدا migration را اجرا کنید

## اضافه کردن نقش جدید

برای اضافه کردن endpointها به نقش جدید:

```sql
DECLARE @NewRoleId INT = (SELECT Id FROM [dbo].[Role] WHERE [Name] = 'Librarian');

INSERT INTO [dbo].[RolePermission] (RoleId, EndpointName)
SELECT @NewRoleId, EndpointName
FROM [dbo].[RolePermission]
WHERE RoleId = (SELECT Id FROM [dbo].[Role] WHERE [Name] = 'Admin')
AND EndpointName IN (
    'Library.Books.Create',
    'Library.Books.Update',
    'Library.Books.Delete'
);
```

