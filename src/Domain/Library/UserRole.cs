namespace Domain.Library;

public sealed class UserRole
{
    public int Id { get; set; }
    public long UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int RoleId { get; set; }
}


