using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Settings.GetAll;

public sealed record GetAllSettingsQuery() : IQuery<List<SettingsResponse>>;

