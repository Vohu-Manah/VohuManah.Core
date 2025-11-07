namespace Application.Library.Users.Login;

public sealed record LoginLibraryUserResponse
{
    public string UserName { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string? AccessToken { get; init; }
    public string? RefreshToken { get; init; }
}


