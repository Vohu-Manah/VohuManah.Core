# راهنمای مدیریت نقش‌ها و دسترسی‌های API از دیتابیس

این راهنما نحوه مدیریت نقش‌ها و دسترسی‌های API از طریق دیتابیس را توضیح می‌دهد.

## ساختار دیتابیس

### جدول Role
نقش‌های سیستم در این جدول نگهداری می‌شوند:
```sql
CREATE TABLE [dbo].[Role] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(50) NOT NULL
);
```

### جدول UserRole
ارتباط بین کاربران و نقش‌ها:
```sql
CREATE TABLE [dbo].[UserRole] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [UserName] NVARCHAR(30) NOT NULL,
    [RoleId] INT NOT NULL,
    FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Role]([Id])
);
```

### جدول RolePermission
**این جدول مشخص می‌کند که هر نقش به کدام endpointها (APIها) دسترسی دارد:**
```sql
CREATE TABLE [dbo].[RolePermission] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [RoleId] INT NOT NULL,
    [EndpointName] NVARCHAR(200) NOT NULL,
    FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Role]([Id]) ON DELETE CASCADE,
    UNIQUE ([RoleId], [EndpointName])
);
```

## نحوه استفاده

### 1. اضافه کردن نقش جدید

```sql
-- اضافه کردن نقش جدید
INSERT INTO [dbo].[Role] ([Name]) VALUES ('Librarian');
```

### 2. اختصاص endpointها به نقش

```sql
-- گرفتن شناسه نقش
DECLARE @RoleId INT = (SELECT Id FROM [dbo].[Role] WHERE [Name] = 'Librarian');

-- اضافه کردن دسترسی به endpointهای مختلف
INSERT INTO [dbo].[RolePermission] ([RoleId], [EndpointName]) VALUES
    (@RoleId, 'Library.Books.Create'),
    (@RoleId, 'Library.Books.Update'),
    (@RoleId, 'Library.Books.Delete'),
    (@RoleId, 'Library.Books.GetById'),
    (@RoleId, 'Library.Books.GetList'),
    (@RoleId, 'Library.Publications.Create'),
    (@RoleId, 'Library.Publications.Update');
```

### 3. اختصاص نقش به کاربر

```sql
-- گرفتن شناسه نقش
DECLARE @RoleId INT = (SELECT Id FROM [dbo].[Role] WHERE [Name] = 'Librarian');

-- اختصاص نقش به کاربر
INSERT INTO [dbo].[UserRole] ([UserName], [RoleId]) VALUES
    ('username', @RoleId);
```

### 4. مشاهده لیست endpointهای یک نقش

```sql
SELECT 
    r.Name AS RoleName,
    rp.EndpointName
FROM [dbo].[RolePermission] rp
INNER JOIN [dbo].[Role] r ON rp.RoleId = r.Id
WHERE r.Name = 'Librarian'
ORDER BY rp.EndpointName;
```

### 5. مشاهده نقش‌های یک کاربر

```sql
SELECT 
    u.UserName,
    r.Name AS RoleName
FROM [dbo].[UserRole] ur
INNER JOIN [dbo].[Role] r ON ur.RoleId = r.Id
INNER JOIN [dbo].[User] u ON ur.UserName = u.UserName
WHERE u.UserName = 'username';
```

### 6. مشاهده endpointهای قابل دسترسی برای یک کاربر

```sql
SELECT DISTINCT
    rp.EndpointName
FROM [dbo].[RolePermission] rp
INNER JOIN [dbo].[UserRole] ur ON rp.RoleId = ur.RoleId
WHERE ur.UserName = 'username'
ORDER BY rp.EndpointName;
```

## فرمت نام endpointها

نام endpointها باید به فرمت زیر باشد:
- `Library.Books.Create`
- `Library.Books.Update`
- `Library.Books.Delete`
- `Library.Books.GetById`
- `Library.Users.Create`
- `Library.Users.Update`
- و غیره...

**الگو:** `Library.{Module}.{Action}`

## Seed کردن داده‌های اولیه

برای اختصاص همه endpointها به نقش Admin:

```sql
-- گرفتن شناسه نقش Admin
DECLARE @AdminRoleId INT = (SELECT Id FROM [dbo].[Role] WHERE [Name] = 'Admin');

-- اضافه کردن همه endpointها به نقش Admin
INSERT INTO [dbo].[RolePermission] ([RoleId], [EndpointName]) VALUES
    (@AdminRoleId, 'Library.Books.Create'),
    (@AdminRoleId, 'Library.Books.Update'),
    (@AdminRoleId, 'Library.Books.Delete'),
    (@AdminRoleId, 'Library.Books.GetById'),
    (@AdminRoleId, 'Library.Books.GetList'),
    (@AdminRoleId, 'Library.Books.GetAll'),
    (@AdminRoleId, 'Library.Books.Search'),
    -- ... بقیه endpointها
    (@AdminRoleId, 'Library.Users.Create'),
    (@AdminRoleId, 'Library.Users.Update'),
    (@AdminRoleId, 'Library.Users.Delete'),
    -- ... و غیره
```

## نکات مهم

1. **همه endpointها باید در جدول RolePermission ثبت شوند** تا سیستم بتواند دسترسی را بررسی کند
2. **اگر endpoint در جدول RolePermission نباشد، هیچ کاربری نمی‌تواند به آن دسترسی داشته باشد**
3. **نقش Admin باید به همه endpointها دسترسی داشته باشد**
4. **نام endpointها باید دقیقاً مطابق با نامی باشد که در attribute `[RequireRole]` استفاده شده است**

## لیست کامل endpointها

برای مشاهده لیست کامل endpointها، فایل‌های موجود در `src/Web.Api/Endpoints/Library` را بررسی کنید.

هر endpoint با attribute `[RequireRole("Library.Xxx.Yyy")]` مشخص شده است که نام endpoint را نشان می‌دهد.

