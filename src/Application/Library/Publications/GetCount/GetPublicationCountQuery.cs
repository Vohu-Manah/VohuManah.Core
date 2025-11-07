using Application.Abstractions.Messaging;

namespace Application.Library.Publications.GetCount;

public sealed record GetPublicationCountQuery : IQuery<int>;

