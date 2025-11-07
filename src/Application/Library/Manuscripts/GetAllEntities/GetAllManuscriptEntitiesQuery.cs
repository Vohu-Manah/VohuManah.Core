using Application.Abstractions.Messaging;
using Domain.Library;

namespace Application.Library.Manuscripts.GetAllEntities;

public sealed record GetAllManuscriptEntitiesQuery : IQuery<List<Manuscript>>;

