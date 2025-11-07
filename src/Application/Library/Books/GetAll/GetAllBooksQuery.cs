using Application.Abstractions.Messaging;

namespace Application.Library.Books.GetAll;

public sealed record GetAllBooksQuery() : IQuery<List<BookResponse>>;
