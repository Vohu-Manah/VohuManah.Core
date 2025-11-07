using Application.Abstractions.Messaging;

namespace Application.Library.Settings.GetByName;

public sealed record GetSettingsByNameQuery(string Name) : IQuery<SettingsResponse>;

