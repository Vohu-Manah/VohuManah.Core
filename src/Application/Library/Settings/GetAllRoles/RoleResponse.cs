namespace Application.Library.Settings.GetAllRoles;

public sealed record RoleResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
}

