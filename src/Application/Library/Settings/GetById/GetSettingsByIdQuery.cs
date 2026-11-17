using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Settings.GetById;

public sealed record GetSettingsByIdQuery(int Id) : IQuery<SettingsResponse>;

