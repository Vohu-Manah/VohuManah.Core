namespace Application.Library.Users.GetCurrentProfile;

public sealed record UserProfileResponse
{
    public long Id { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }
}

