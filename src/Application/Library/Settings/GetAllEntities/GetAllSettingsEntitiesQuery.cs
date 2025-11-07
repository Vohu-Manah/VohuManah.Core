using Application.Abstractions.Messaging;
using Domain.Library;

namespace Application.Library.Settings.GetAllEntities;

public sealed record GetAllSettingsEntitiesQuery : IQuery<List<Domain.Library.Settings>>;

