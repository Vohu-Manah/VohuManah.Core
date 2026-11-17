using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Settings.GetByName;

public sealed record GetSettingsByNameQuery(string Name) : IQuery<SettingsResponse>;

