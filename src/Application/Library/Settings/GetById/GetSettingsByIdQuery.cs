using Application.Abstractions.Messaging;

namespace Application.Library.Settings.GetById;

public sealed record GetSettingsByIdQuery(int Id) : IQuery<SettingsResponse>;

