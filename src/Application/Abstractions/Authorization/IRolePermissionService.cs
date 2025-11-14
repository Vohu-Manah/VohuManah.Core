namespace Application.Abstractions.Authorization;

/// <summary>
/// سرویس برای بررسی دسترسی نقش‌ها به endpointها
/// </summary>
public interface IRolePermissionService
{
    /// <summary>
    /// بررسی می‌کند که آیا یک نقش به endpoint مشخص شده دسترسی دارد
    /// </summary>
    Task<bool> HasPermissionAsync(string roleName, string endpointName, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// لیست تمام endpointهای یک نقش را برمی‌گرداند
    /// </summary>
    Task<List<string>> GetEndpointsForRoleAsync(string roleName, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// لیست تمام نقش‌هایی که به یک endpoint دسترسی دارند را برمی‌گرداند
    /// </summary>
    Task<List<string>> GetRolesForEndpointAsync(string endpointName, CancellationToken cancellationToken = default);
}

