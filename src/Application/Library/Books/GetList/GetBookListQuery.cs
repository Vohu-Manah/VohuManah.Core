using Application.Abstractions.Messaging;

namespace Application.Library.Books.GetList;

public sealed record GetBookListQuery : IQuery<List<BookListResponse>>;

