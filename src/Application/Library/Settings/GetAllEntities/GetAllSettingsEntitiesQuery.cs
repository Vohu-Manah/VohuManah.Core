using Application.Abstractions.Messaging;

namespace Application.Library.Settings.GetAllEntities;

public sealed record GetAllSettingsEntitiesQuery : IQuery<List<Domain.Library.Settings>>;

