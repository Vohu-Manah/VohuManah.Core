namespace Application.Library.Settings.GetAppInfo;

public sealed record AppInfoResponse
{
    public int Id { get; init; }
    public string AppName { get; init; } = string.Empty;
    public string Version { get; init; } = string.Empty;
    public string Author { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateTime? LastUpdateDate { get; init; }
    public string? Copyright { get; init; }
    public string? License { get; init; }
    public string? Website { get; init; }
    public string? SupportEmail { get; init; }
}


