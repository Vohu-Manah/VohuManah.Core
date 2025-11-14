# Clean Architecture Template

A comprehensive, production-ready Clean Architecture template built with .NET 8, featuring modern development practices and enterprise-grade solutions.

## 🏗️ Architecture Overview

This template implements Clean Architecture principles with clear separation of concerns across multiple layers:

- **Domain Layer**: Core business logic and entities
- **Application Layer**: Use cases and business rules
- **Infrastructure Layer**: External concerns and data access
- **Presentation Layer**: Web API and UI components

## 🚀 Features

### Core Architecture
- ✅ **Clean Architecture** with proper dependency inversion
- ✅ **CQRS** (Command Query Responsibility Segregation)
- ✅ **SOLID Principles** implementation
- ✅ **Dependency Injection** throughout the application

### Backend Technologies
- ✅ **.NET 8** with C# 12 features
- ✅ **ASP.NET Core Web API** with minimal APIs
- ✅ **Entity Framework Core** with PostgreSQL
- ✅ **JWT Authentication** and authorization
- ✅ **Permission-based authorization** system
- ✅ **Serilog** for structured logging
- ✅ **Health Checks** for monitoring
- ✅ **Swagger/OpenAPI** documentation

### Frontend Technologies
- ✅ **React** with modern hooks
- ✅ **Vite** for fast development and building
- ✅ **TypeScript** for type safety
- ✅ **Tailwind CSS** for styling
- ✅ **React Query** for data fetching
- ✅ **React Router** for navigation
- ✅ **Docker** containerization

### Infrastructure & DevOps
- ✅ **Docker** and **Docker Compose** setup
- ✅ **PostgreSQL** database
- ✅ **Seq** for log aggregation (http://localhost:8081)
- ✅ **Redis** for caching (optional)
- ✅ **Health monitoring** endpoints

### Testing
- ✅ **Unit Testing** with xUnit
- ✅ **Integration Testing** setup
- ✅ **Architecture Testing** with NetArchTest
- ✅ **Test containers** for database testing

## 📁 Project Structure

```
src/
├── Domain/                 # Domain layer - Core business logic
├── Application/           # Application layer - Use cases and interfaces
├── Infrastructure/        # Infrastructure layer - External concerns
│   ├── Caching/          # Cache management with dependency injection
│   ├── Helper/           # Utility classes and Persian date utilities
│   ├── Database/         # Entity Framework and database context
│   ├── Authentication/   # JWT authentication
│   └── Authorization/    # Permission-based authorization
├── Web.Api/              # Web API layer
└── Web.UI/               # React frontend application

tests/
└── ArchitectureTests/    # Architecture compliance tests
```

## 🛠️ Getting Started

### Prerequisites
- .NET 8 SDK
- Node.js 18+ and npm
- Docker and Docker Compose
- PostgreSQL (or use Docker)

### Quick Start

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd VohuManah
   ```

2. **Start with Docker Compose**
   ```bash
   docker-compose up -d
   ```

3. **Run the application**
   ```bash
   # Backend
   dotnet run --project src/Web.Api
   
   # Frontend (in another terminal)
   cd src/Web.UI
   npm install
   npm run dev
   ```

4. **Access the applications**
   - API: http://localhost:5000
   - Frontend: http://localhost:3000
   - Seq Logs: http://localhost:8081
   - Swagger: http://localhost:5000/swagger

## 🔧 Development

### Backend Development
```bash
# Restore packages
dotnet restore

# Run tests
dotnet test

# Build solution
dotnet build

# Run specific project
dotnet run --project src/Web.Api
```

### Frontend Development
```bash
cd src/Web.UI

# Install dependencies
npm install

# Start development server
npm run dev

# Build for production
npm run build
```

### Database Management
```bash
# Add migration
dotnet ef migrations add MigrationName --project src/Infrastructure --startup-project src/Web.Api

# Update database
dotnet ef database update --project src/Infrastructure --startup-project src/Web.Api
```

## 🏛️ Architecture Patterns

### Dependency Injection
All services are properly registered with appropriate lifetimes:
- **Singleton**: Cache managers, configuration services
- **Scoped**: Database context, business services
- **Transient**: Use cases, handlers

### Caching Strategy
- **Memory Cache**: For application-level caching
- **Redis Cache**: For distributed caching (optional)
- **Cache Invalidation**: Pattern-based cache removal

### Authentication & Authorization
- **JWT Tokens**: Stateless authentication
- **Role-based Access**: User roles and permissions
- **Permission Guards**: Fine-grained authorization

## 🧪 Testing Strategy

### Unit Tests
- Domain logic testing
- Use case testing
- Service layer testing

### Integration Tests
- API endpoint testing
- Database integration
- External service mocking

### Architecture Tests
- Dependency direction validation
- Naming convention enforcement
- Layer isolation verification

## 📊 Monitoring & Logging

### Structured Logging
- **Serilog** with structured logging
- **Seq** for log aggregation and analysis
- **Health checks** for application monitoring

### Performance Monitoring
- Request/response logging
- Database query monitoring
- Cache hit/miss tracking

## 🚀 Deployment

### Docker Deployment
```bash
# Build and run with Docker Compose
docker-compose up -d

# Scale services
docker-compose up -d --scale web.api=3
```

### Production Considerations
- Environment-specific configurations
- Database connection pooling
- Caching strategies
- Security hardening
- Performance optimization

## 🔒 Security Features

- JWT token authentication
- Permission-based authorization
- Input validation and sanitization
- CORS configuration
- HTTPS enforcement
- SQL injection prevention

## 📈 Performance Optimizations

- Entity Framework query optimization
- Caching strategies
- Async/await patterns
- Connection pooling
- Response compression


This project is licensed under the MIT License - see the LICENSE file for details.

## 🙏 Acknowledgments

- Clean Architecture principles by Robert C. Martin
- Domain-Driven Design by Eric Evans
- .NET community for excellent tooling and libraries


# تمپلیت معماری کلین

یک تمپلیت جامع و آماده تولید با معماری کلین که با .NET 8 ساخته شده و شامل روش‌های توسعه مدرن و راه‌حل‌های سطح سازمانی است.

## 🏗️ نمای کلی معماری

این تمپلیت اصول معماری کلین را با جداسازی واضح نگرانی‌ها در چندین لایه پیاده‌سازی می‌کند:

- **لایه دامنه**: منطق کسب‌وکار اصلی و موجودیت‌ها
- **لایه کاربرد**: موارد استفاده و قوانین کسب‌وکار
- **لایه زیرساخت**: نگرانی‌های خارجی و دسترسی به داده
- **لایه ارائه**: Web API و اجزای رابط کاربری

## 🚀 ویژگی‌ها

### معماری اصلی
- ✅ **معماری تمیز** با وارونگی وابستگی مناسب
- ✅ **CQRS** (جداسازی مسئولیت دستور و پرس‌وجو)
- ✅ **پیاده‌سازی اصول SOLID**
- ✅ **تزریق وابستگی** در سراسر برنامه

### فناوری‌های بک‌اند
- ✅ **.NET 8** با ویژگی‌های C# 12
- ✅ **ASP.NET Core Web API** با minimal APIs
- ✅ **Entity Framework Core** با PostgreSQL
- ✅ **احراز هویت JWT** و مجوزدهی
- ✅ **سیستم مجوزدهی مبتنی بر مجوز**
- ✅ **Serilog** برای لاگ‌گیری ساختاریافته
- ✅ **بررسی‌های سلامت** برای نظارت
- ✅ **مستندات Swagger/OpenAPI**

### فناوری‌های فرانت‌اند
- ✅ **React** با هوک‌های مدرن
- ✅ **Vite** برای توسعه و ساخت سریع
- ✅ **TypeScript** برای ایمنی نوع
- ✅ **Tailwind CSS** برای استایل‌دهی
- ✅ **React Query** برای دریافت داده
- ✅ **React Router** برای ناوبری
- ✅ **کانتینرسازی Docker**

### زیرساخت و DevOps
- ✅ **راه‌اندازی Docker و Docker Compose**
- ✅ **پایگاه داده PostgreSQL**
- ✅ **Seq** برای تجمیع لاگ (http://localhost:8081)
- ✅ **Redis** برای کش (اختیاری)
- ✅ **نقاط پایانی نظارت بر سلامت**

### تست
- ✅ **تست واحد** با xUnit
- ✅ **راه‌اندازی تست یکپارچگی**
- ✅ **تست معماری** با NetArchTest
- ✅ **کانتینرهای تست** برای تست پایگاه داده

## 📁 ساختار پروژه

```
src/
├── Domain/                 # لایه دامنه - منطق کسب‌وکار اصلی
├── Application/           # لایه کاربرد - موارد استفاده و رابط‌ها
├── Infrastructure/        # لایه زیرساخت - نگرانی‌های خارجی
│   ├── Caching/          # مدیریت کش با تزریق وابستگی
│   ├── Helper/           # کلاس‌های کمکی و ابزارهای تاریخ شمسی
│   ├── Database/         # Entity Framework و زمینه پایگاه داده
│   ├── Authentication/   # احراز هویت JWT
│   └── Authorization/    # مجوزدهی مبتنی بر مجوز
├── Web.Api/              # لایه Web API
└── Web.UI/               # برنامه فرانت‌اند React

tests/
└── ArchitectureTests/    # تست‌های انطباق معماری
```

## 🛠️ شروع کار

### پیش‌نیازها
- .NET 8 SDK
- Node.js 18+ و npm
- Docker و Docker Compose
- PostgreSQL (یا از Docker استفاده کنید)

### شروع سریع

1. **کلون کردن مخزن**
   ```bash
   git clone <repository-url>
   cd VohuManah
   ```

2. **شروع با Docker Compose**
   ```bash
   docker-compose up -d
   ```

3. **اجرای برنامه**
   ```bash
   # بک‌اند
   dotnet run --project src/Web.Api
   
   # فرانت‌اند (در ترمینال دیگر)
   cd src/Web.UI
   npm install
   npm run dev
   ```

4. **دسترسی به برنامه‌ها**
   - API: http://localhost:5000
   - فرانت‌اند: http://localhost:3000
   - لاگ‌های Seq: http://localhost:8081
   - Swagger: http://localhost:5000/swagger

## 🔧 توسعه

### توسعه بک‌اند
```bash
# بازگردانی پکیج‌ها
dotnet restore

# اجرای تست‌ها
dotnet test

# ساخت راه‌حل
dotnet build

# اجرای پروژه خاص
dotnet run --project src/Web.Api
```

### توسعه فرانت‌اند
```bash
cd src/Web.UI

# نصب وابستگی‌ها
npm install

# شروع سرور توسعه
npm run dev

# ساخت برای تولید
npm run build
```

### مدیریت پایگاه داده
```bash
# افزودن migration
dotnet ef migrations add MigrationName --project src/Infrastructure --startup-project src/Web.Api

# به‌روزرسانی پایگاه داده
dotnet ef database update --project src/Infrastructure --startup-project src/Web.Api
```

## 🏛️ الگوهای معماری

### تزریق وابستگی
تمام سرویس‌ها با طول عمر مناسب ثبت شده‌اند:
- **Singleton**: مدیران کش، سرویس‌های پیکربندی
- **Scoped**: زمینه پایگاه داده، سرویس‌های کسب‌وکار
- **Transient**: موارد استفاده، پردازنده‌ها

### استراتژی کش
- **کش حافظه**: برای کش سطح برنامه
- **کش Redis**: برای کش توزیع‌شده (اختیاری)
- **ابطال کش**: حذف کش مبتنی بر الگو

### احراز هویت و مجوزدهی
- **توکن‌های JWT**: احراز هویت بدون حالت
- **دسترسی مبتنی بر نقش**: نقش‌ها و مجوزهای کاربر
- **نگهبان‌های مجوز**: مجوزدهی دقیق

## 🧪 استراتژی تست

### تست‌های واحد
- تست منطق دامنه
- تست موارد استفاده
- تست لایه سرویس

### تست‌های یکپارچگی
- تست نقاط پایانی API
- یکپارچگی پایگاه داده
- شبیه‌سازی سرویس‌های خارجی

### تست‌های معماری
- اعتبارسنجی جهت وابستگی
- اجرای قراردادهای نام‌گذاری
- تأیید جداسازی لایه

## 📊 نظارت و لاگ‌گیری

### لاگ‌گیری ساختاریافته
- **Serilog** با لاگ‌گیری ساختاریافته
- **Seq** برای تجمیع و تحلیل لاگ
- **بررسی‌های سلامت** برای نظارت بر برنامه

### نظارت بر عملکرد
- لاگ‌گیری درخواست/پاسخ
- نظارت بر پرس‌وجوهای پایگاه داده
- ردیابی ضربه/خطای کش

## 🚀 استقرار

### استقرار Docker
```bash
# ساخت و اجرا با Docker Compose
docker-compose up -d

# مقیاس‌دهی سرویس‌ها
docker-compose up -d --scale web.api=3
```

### ملاحظات پرداکشن
- پیکربندی‌های خاص محیط
- استخر اتصال پایگاه داده
- استراتژی‌های کش
- سخت‌سازی امنیتی
- بهینه‌سازی عملکرد

## 🔒 ویژگی‌های امنیتی

- احراز هویت توکن JWT
- مجوزدهی مبتنی بر مجوز
- اعتبارسنجی و پاک‌سازی ورودی
- پیکربندی CORS
- اجرای HTTPS
- پیشگیری از تزریق SQL

## 📈 بهینه‌سازی‌های عملکرد

- بهینه‌سازی پرس‌وجو Entity Framework
- استراتژی‌های کش
- الگوهای async/await
- استخر اتصال
- فشرده‌سازی پاسخ

## 🙏 تشکر

- اصول معماری تمیز توسط Robert C. Martin
- جامعه .NET برای ابزارها و کتابخانه‌های عالی

