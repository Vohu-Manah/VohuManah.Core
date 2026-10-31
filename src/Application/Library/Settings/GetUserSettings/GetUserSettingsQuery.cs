using Application.Abstractions.Messaging;

namespace Application.Library.Settings.GetUserSettings;

public sealed record GetUserSettingsQuery(long UserId) : IQuery<UserSettingsResponse>;


