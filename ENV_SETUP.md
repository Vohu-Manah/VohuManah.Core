# راهنمای تنظیم Environment Variables

## فایل‌های Environment

- **`.env`**: فایل اصلی که باید ایجاد کنید و مقادیر واقعی را در آن قرار دهید (این فایل در Git commit نمی‌شود)
- **`.env.settings`**: فایل template که به عنوان نمونه استفاده می‌شود (این فایل در Git commit می‌شود)

## مراحل راه‌اندازی

### 1. ایجاد فایل `.env`

از فایل template یک کپی ایجاد کنید:

```powershell
Copy-Item .env.settings .env
```

یا در Linux/Mac:

```bash
cp .env.settings .env
```

### 2. ویرایش فایل `.env`

فایل `.env` را باز کنید و مقادیر را مطابق با محیط خود تنظیم کنید:

```env
# SQL Server Configuration
SQL_SERVER_HOST=sql-server
SQL_SERVER_PORT=1433
SQL_SERVER_DATABASE=Library
SQL_SERVER_USER=sa
SQL_SERVER_PASSWORD=YourStrong@Passw0rd

# Application Configuration
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_HTTP_PORTS=5000
ASPNETCORE_HTTPS_PORTS=5001

# Docker Registry (optional)
DOCKER_REGISTRY=
```

### 3. استفاده از Docker Compose

Docker Compose به صورت خودکار فایل `.env` را می‌خواند. فقط کافی است:

```powershell
docker-compose up -d
```

## متغیرهای موجود

### SQL Server Configuration

- `SQL_SERVER_HOST`: آدرس SQL Server (برای Docker container از `sql-server` استفاده کنید)
- `SQL_SERVER_PORT`: پورت SQL Server (پیش‌فرض: `1433`)
- `SQL_SERVER_DATABASE`: نام دیتابیس (پیش‌فرض: `Library`)
- `SQL_SERVER_USER`: نام کاربری SQL Server (پیش‌فرض: `sa`)
- `SQL_SERVER_PASSWORD`: رمز عبور SQL Server (**حتماً تغییر دهید!**)

### Application Configuration

- `ASPNETCORE_ENVIRONMENT`: محیط اجرا (Development, Staging, Production)
- `ASPNETCORE_HTTP_PORTS`: پورت HTTP (پیش‌فرض: `5000`)
- `ASPNETCORE_HTTPS_PORTS`: پورت HTTPS (پیش‌فرض: `5001`)

### Docker Registry (اختیاری)

- `DOCKER_REGISTRY`: آدرس Docker Registry در صورت استفاده

## نکات امنیتی

⚠️ **مهم**: 
- هرگز فایل `.env` را در Git commit نکنید
- فایل `.env` در `.gitignore` قرار دارد
- برای production، از یک سیستم مدیریت secrets استفاده کنید
- رمز عبور SQL Server را حتماً تغییر دهید

## استفاده در Production

برای محیط production:
1. فایل `.env` را با مقادیر production ایجاد کنید
2. از یک سیستم مدیریت secrets استفاده کنید (مثل Azure Key Vault, AWS Secrets Manager)
3. رمزهای عبور قوی استفاده کنید
4. دسترسی به فایل `.env` را محدود کنید

