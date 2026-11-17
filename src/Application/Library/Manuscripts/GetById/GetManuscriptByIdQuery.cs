using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Manuscripts.GetById;

public sealed record GetManuscriptByIdQuery(int Id) : IQuery<ManuscriptResponse>;

