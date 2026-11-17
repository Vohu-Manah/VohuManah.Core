using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Settings.UpdateUserSettings;

public sealed record UpdateUserSettingsCommand(
    long UserId,
    long? ProfileImageAttachmentId,
    string? ThemePrimaryColor,
    string? ThemeSecondaryColor,
    string? ThemeAccentColor,
    string? ThemeMode) : ICommand;

