using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Settings.GetUserSettings;

public sealed record GetUserSettingsQuery(long UserId) : IQuery<UserSettingsResponse>;


