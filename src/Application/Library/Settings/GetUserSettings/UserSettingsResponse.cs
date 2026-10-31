namespace Application.Library.Settings.GetUserSettings;

public sealed record UserSettingsResponse
{
    public long Id { get; init; }
    public long UserId { get; init; }
    public long? ProfileImageAttachmentId { get; init; }
    public string? ProfileImageUrl { get; init; }
    public string? ThemePrimaryColor { get; init; }
    public string? ThemeSecondaryColor { get; init; }
    public string? ThemeAccentColor { get; init; }
    public string? ThemeMode { get; init; }
}

