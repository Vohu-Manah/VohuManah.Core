using Application.Abstractions.Messaging;

namespace Application.Library.Settings.UpdateUserSettings;

public sealed record UpdateUserSettingsCommand(
    long UserId,
    long? ProfileImageAttachmentId,
    string? ThemePrimaryColor,
    string? ThemeSecondaryColor,
    string? ThemeAccentColor,
    string? ThemeMode) : ICommand;

