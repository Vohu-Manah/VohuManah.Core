using Application.Abstractions.Messaging;

namespace Application.Library.Manuscripts.GetCount;

public sealed record GetManuscriptCountQuery : IQuery<int>;

