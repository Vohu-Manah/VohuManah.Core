namespace Application.Library.Users.GetList;

public sealed record UserListResponse
{
    public string UserName { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
}

