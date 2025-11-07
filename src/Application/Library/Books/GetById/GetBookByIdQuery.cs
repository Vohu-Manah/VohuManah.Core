using Application.Abstractions.Messaging;

namespace Application.Library.Books.GetById;

public sealed record GetBookByIdQuery(long Id) : IQuery<BookResponse>;

