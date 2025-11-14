namespace Web.Api.Endpoints.Attributes;

/// <summary>
/// Attribute برای تعریف endpoint و بررسی دسترسی از دیتابیس
/// این attribute را روی کلاس endpoint قرار دهید
/// نام endpoint برای بررسی دسترسی از جدول RolePermission استفاده می‌شود
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class RequireRoleAttribute : Attribute
{
    /// <summary>
    /// نام endpoint برای بررسی دسترسی از دیتابیس
    /// مثال: "Library.Books.Create", "Library.Users.Update"
    /// </summary>
    public string EndpointName { get; }

    /// <summary>
    /// اگر true باشد، endpoint از دیتابیس خوانده نمی‌شود و فقط احراز هویت بررسی می‌شود
    /// </summary>
    public bool SkipDatabaseCheck { get; set; }

    /// <summary>
    /// نقش‌های مورد نیاز (برای backward compatibility - استفاده نمی‌شود)
    /// </summary>
#pragma warning disable S1133 // Do not forget to remove this deprecated code someday
    [Obsolete("استفاده از Roles منسوخ شده است. از EndpointName استفاده کنید.")]
    public string[] Roles { get; }
#pragma warning restore S1133 // Do not forget to remove this deprecated code someday

    /// <summary>
    /// اگر true باشد، کاربر باید همه نقش‌های مشخص شده را داشته باشد (برای backward compatibility)
    /// </summary>
#pragma warning disable S1133 // Do not forget to remove this deprecated code someday
    [Obsolete("استفاده از RequireAll منسوخ شده است.")]
    public bool RequireAll { get; set; }
#pragma warning restore S1133 // Do not forget to remove this deprecated code someday

    /// <summary>
    /// Constructor با نام endpoint
    /// </summary>
    public RequireRoleAttribute(string endpointName)
    {
        EndpointName = endpointName ?? throw new ArgumentNullException(nameof(endpointName));
#pragma warning disable CS0618 // Type or member is obsolete
        Roles = Array.Empty<string>();
#pragma warning restore CS0618 // Type or member is obsolete
    }

    /// <summary>
    /// Constructor قدیمی برای backward compatibility
    /// </summary>
#pragma warning disable S1133 // Do not forget to remove this deprecated code someday
    [Obsolete("از constructor با endpointName استفاده کنید.")]
    public RequireRoleAttribute(params string[] roles)
#pragma warning restore S1133 // Do not forget to remove this deprecated code someday
    {
        Roles = roles ?? throw new ArgumentNullException(nameof(roles));
        EndpointName = string.Empty;
    }
}

