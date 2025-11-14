# راهنمای استفاده از سیستم مجوزدهی مبتنی بر نقش

این راهنما نحوه استفاده از سیستم مرکزی مجوزدهی مبتنی بر نقش را توضیح می‌دهد.

## نحوه استفاده

### روش 1: استفاده از BaseEndpoint (توصیه می‌شود)

برای استفاده از این روش، کلاس endpoint خود را از `BaseEndpoint` ارث‌بری کنید و attribute `[RequireRole]` را روی کلاس قرار دهید:

```csharp
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;

[RequireRole("Admin")]
internal sealed class Update : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("library/users", async (...) => { ... })
            .ApplyRoleAuthorization()  // نقش‌ها به صورت خودکار از attribute اعمال می‌شوند
            .WithTags("Library.Users");
    }
}
```

### روش 2: استفاده مستقیم از IEndpoint

اگر نمی‌خواهید از `BaseEndpoint` استفاده کنید، می‌توانید مستقیماً از extension method استفاده کنید:

```csharp
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;

[RequireRole("Admin", "Librarian")]
internal sealed class Delete : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("library/books/{id}", async (...) => { ... })
            .ApplyRoleAuthorization(this)  // نقش‌ها از attribute خوانده می‌شوند
            .WithTags("Library.Books");
    }
}
```

## انواع استفاده از RequireRoleAttribute

### نیاز به یک نقش

```csharp
[RequireRole("Admin")]
internal sealed class Update : BaseEndpoint
{
    // فقط کاربران با نقش Admin می‌توانند به این endpoint دسترسی داشته باشند
}
```

### نیاز به یکی از چند نقش

```csharp
[RequireRole("Admin", "Librarian")]
internal sealed class Delete : BaseEndpoint
{
    // کاربران با نقش Admin یا Librarian می‌توانند به این endpoint دسترسی داشته باشند
}
```

### نیاز به همه نقش‌ها

```csharp
[RequireRole("Admin", "SuperUser", RequireAll = true)]
internal sealed class SensitiveOperation : BaseEndpoint
{
    // کاربر باید هم Admin و هم SuperUser باشد
}
```

### بدون نیاز به نقش (فقط احراز هویت)

```csharp
internal sealed class GetProfile : BaseEndpoint
{
    // اگر attribute نباشد، فقط احراز هویت کافی است
    // (از RouteGroupBuilder که در Program.cs تعریف شده)
}
```

## مزایای این روش

1. **مرکزی بودن**: همه نقش‌ها در یک جا (روی کلاس endpoint) تعریف می‌شوند
2. **خوانایی بهتر**: کد تمیزتر و قابل فهم‌تر می‌شود
3. **نگهداری آسان**: تغییر نقش‌ها فقط در یک جا انجام می‌شود
4. **کاهش خطا**: احتمال فراموش کردن تعریف نقش کمتر می‌شود

## فایل مرکزی مدیریت نقش‌ها

همه نقش‌ها و endpointهای مربوطه در فایل `EndpointRoleConfiguration.cs` مدیریت می‌شوند.

### موقعیت فایل
```
src/Web.Api/Endpoints/EndpointRoleConfiguration.cs
```

### نحوه اضافه کردن نقش جدید

1. **نقش را در دیتابیس اضافه کنید:**
   ```sql
   INSERT INTO [dbo].[Role] ([Name]) VALUES ('Librarian')
   ```

2. **در فایل `EndpointRoleConfiguration.cs`، نقش و endpointهای مربوطه را مشخص کنید:**
   ```csharp
   public static readonly Dictionary<string, string[]> RoleEndpointMapping = new()
   {
       { "Librarian", new[] { 
           "Library.Books.Create", 
           "Library.Books.Update", 
           "Library.Books.Delete"
       } }
   };
   ```

3. **در endpointهای مربوطه، attribute را تغییر دهید:**
   ```csharp
   [RequireRole("Librarian")]  // به جای "Admin"
   internal sealed class Create : BaseEndpoint
   {
       // ...
   }
   ```

### لیست کامل endpointها

همه endpointهای موجود در پروژه به صورت پیش‌فرض با نقش "Admin" محافظت می‌شوند. برای مشاهده لیست کامل endpointها، فایل `EndpointRoleConfiguration.cs` را بررسی کنید.

## نکات مهم

- اگر `RequireRoleAttribute` روی کلاس نباشد، فقط احراز هویت (authentication) بررسی می‌شود
- نقش‌ها از JWT token که در `LibraryTokenProvider` ایجاد می‌شود خوانده می‌شوند
- نقش‌های پیش‌فرض در دیتابیس: "Admin" و "User"
- همه endpointها (به جز Login و RefreshToken) به صورت پیش‌فرض با نقش "Admin" محافظت می‌شوند
- برای تغییر نقش endpointها، فایل `EndpointRoleConfiguration.cs` را ویرایش کنید

