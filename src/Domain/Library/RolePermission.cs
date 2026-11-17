using System.Diagnostics.CodeAnalysis;

namespace VohuManah.Domain.Library;

[SuppressMessage("", "CA1711")]
public sealed class RolePermission
{
    public int Id { get; set; }

    public int RoleId { get; set; }

    public string EndpointName { get; set; } = string.Empty;

    public Role? Role { get; set; }
}

