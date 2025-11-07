using Application.Abstractions.Messaging;

namespace Application.Library.Manuscripts.GetAll;

public sealed record GetAllManuscriptsQuery() : IQuery<List<ManuscriptResponse>>;

