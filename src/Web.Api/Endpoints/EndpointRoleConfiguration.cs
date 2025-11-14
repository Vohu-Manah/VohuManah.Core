using System.Collections.Immutable;
using System.Collections.Generic;

namespace Web.Api.Endpoints;

/// <summary>
/// فایل مرکزی برای تعریف نقش‌ها و endpointهای مربوط به هر نقش
/// 
/// نحوه استفاده:
/// 1. همه endpointها به صورت پیش‌فرض با نقش "Admin" محافظت می‌شوند
/// 2. برای اضافه کردن نقش جدید:
///    - نقش را در دیتابیس اضافه کنید (جدول Role)
///    - در این فایل، در RoleEndpointMapping، نقش و endpointهای مربوطه را مشخص کنید
///    - endpointهای مربوطه را ویرایش کنید و [RequireRole("نقش جدید")] را اضافه کنید
/// 
/// مثال:
/// اگر می‌خواهید نقش "Librarian" ایجاد کنید و به endpointهای Books دسترسی بدهید:
/// 1. در دیتابیس نقش "Librarian" را اضافه کنید
/// 2. در RoleEndpointMapping اضافه کنید:
///    { "Librarian", new[] { 
///        "Library.Books.Create", 
///        "Library.Books.Update", 
///        "Library.Books.Delete"
///    } }
/// 3. در endpointهای مربوطه، [RequireRole("Librarian")] را جایگزین [RequireRole("Admin")] کنید
/// </summary>
public static class EndpointRoleConfiguration
{
    /// <summary>
    /// نقش پیش‌فرض برای همه endpointها (به جز endpointهای AllowAnonymous)
    /// </summary>
    public const string DefaultRole = "Admin";

    /// <summary>
    /// لیست endpointهایی که باید AllowAnonymous باشند (نیازی به احراز هویت ندارند)
    /// </summary>
    public static readonly ImmutableHashSet<string> AnonymousEndpoints = ImmutableHashSet.Create(
        "Login",
        "RefreshToken");

    /// <summary>
    /// Mapping نقش‌ها به endpointها
    /// 
    /// برای اضافه کردن نقش جدید:
    /// 1. نقش را در دیتابیس اضافه کنید
    /// 2. در این dictionary، نقش و endpointهای مربوطه را مشخص کنید
    /// 3. endpointهای مربوطه را ویرایش کنید و [RequireRole("نقش جدید")] را اضافه کنید
    /// 
    /// نام endpointها باید به صورت کامل باشد، مثلاً:
    /// - "Library.Books.Create"
    /// - "Library.Users.Update"
    /// - "Library.Publications.Delete"
    /// </summary>
    public static readonly ImmutableDictionary<string, ImmutableArray<string>> RoleEndpointMapping = ImmutableDictionary<string, ImmutableArray<string>>.Empty;
        // نقش Admin به صورت پیش‌فرض به همه endpointها دسترسی دارد
        // برای نقش‌های دیگر، endpointهای خاص را مشخص کنید:
        
        // ============================================
        // مثال: نقش Librarian
        // ============================================
        // RoleEndpointMapping = RoleEndpointMapping.Add("Librarian", ImmutableArray.Create(
        //     "Library.Books.Create", 
        //     "Library.Books.Update", 
        //     "Library.Books.Delete",
        //     "Library.Books.GetById",
        //     "Library.Books.GetList",
        //     "Library.Books.Search",
        //     "Library.Publications.Create",
        //     "Library.Publications.Update",
        //     "Library.Publications.Delete"));
        
        // ============================================
        // مثال: نقش User (دسترسی فقط خواندن)
        // ============================================
        // RoleEndpointMapping = RoleEndpointMapping.Add("User", ImmutableArray.Create(
        //     "Library.Books.GetById", 
        //     "Library.Books.GetList",
        //     "Library.Books.Search",
        //     "Library.Publications.GetById",
        //     "Library.Publications.GetList",
        //     "Library.Publications.Search"));

    /// <summary>
    /// بررسی می‌کند که آیا endpoint باید AllowAnonymous باشد
    /// </summary>
    public static bool IsAnonymous(string endpointName)
    {
        return AnonymousEndpoints.Contains(endpointName);
    }

    /// <summary>
    /// نقش‌های مورد نیاز برای یک endpoint را برمی‌گرداند
    /// </summary>
    public static string[] GetRolesForEndpoint(string endpointFullName)
    {
        // ابتدا بررسی می‌کنیم که آیا endpoint در mapping نقش خاصی دارد
        foreach (KeyValuePair<string, ImmutableArray<string>> mapping in RoleEndpointMapping)
        {
            if (mapping.Value.Contains(endpointFullName))
            {
                return new[] { mapping.Key };
            }
        }

        // در غیر این صورت، نقش پیش‌فرض را برمی‌گردانیم
        return new[] { DefaultRole };
    }
}

