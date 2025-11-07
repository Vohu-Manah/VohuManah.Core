using Application.Abstractions.Messaging;
using Domain.Library;

namespace Application.Library.Publications.GetAllEntities;

public sealed record GetAllPublicationEntitiesQuery : IQuery<List<Publication>>;

