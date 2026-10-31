namespace Domain.Library;


public sealed class RoleEndpoint
{
    public int Id { get; set; }

    public int RoleId { get; set; }

    public string EndpointName { get; set; } = string.Empty;

    public Role? Role { get; set; }
}

