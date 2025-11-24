namespace Application.Library.Users.GetByUserName;

public sealed record LibraryUserResponse
{
    public long Id { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
}


