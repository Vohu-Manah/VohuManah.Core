using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Settings.GetAllEntities;

public sealed record GetAllSettingsEntitiesQuery : IQuery<List<VohuManah.Domain.AppSettings.Settings>>;

