using Application.Abstractions.Messaging;

namespace Application.Library.Manuscripts.GetById;

public sealed record GetManuscriptByIdQuery(int Id) : IQuery<ManuscriptResponse>;

