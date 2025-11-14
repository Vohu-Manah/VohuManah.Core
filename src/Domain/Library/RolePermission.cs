namespace Domain.Library;

/// <summary>
/// جدول ارتباط بین نقش‌ها و endpointها (APIها)
/// این جدول مشخص می‌کند که هر نقش به کدام endpointها دسترسی دارد
/// </summary>
public sealed class RolePermission
{
    public int Id { get; set; }
    
    /// <summary>
    /// شناسه نقش
    /// </summary>
    public int RoleId { get; set; }
    
    /// <summary>
    /// نام endpoint (مثلاً: Library.Books.Create)
    /// </summary>
    public string EndpointName { get; set; } = string.Empty;
    
    /// <summary>
    /// Navigation property به Role
    /// </summary>
    public Role? Role { get; set; }
}

