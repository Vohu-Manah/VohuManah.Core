using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Library;

namespace VohuManah.Application.Library.Publications.GetAllEntities;

public sealed record GetAllPublicationEntitiesQuery : IQuery<List<Publication>>;

