using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Books.GetAll;

public sealed record GetAllBooksQuery() : IQuery<List<BookResponse>>;
