using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Manuscripts;

namespace VohuManah.Application.Library.Manuscripts.GetAllEntities;

public sealed record GetAllManuscriptEntitiesQuery : IQuery<List<Manuscript>>;

