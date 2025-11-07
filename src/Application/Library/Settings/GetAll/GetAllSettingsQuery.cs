using Application.Abstractions.Messaging;

namespace Application.Library.Settings.GetAll;

public sealed record GetAllSettingsQuery() : IQuery<List<SettingsResponse>>;

