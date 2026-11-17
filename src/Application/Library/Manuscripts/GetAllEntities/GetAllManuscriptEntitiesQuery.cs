using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Library;

namespace VohuManah.Application.Library.Manuscripts.GetAllEntities;

public sealed record GetAllManuscriptEntitiesQuery : IQuery<List<Manuscript>>;

