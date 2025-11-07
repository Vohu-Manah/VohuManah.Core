using Application.Abstractions.Messaging;

namespace Application.Library.Books.GetCount;

public sealed record GetBookCountQuery : IQuery<int>;

