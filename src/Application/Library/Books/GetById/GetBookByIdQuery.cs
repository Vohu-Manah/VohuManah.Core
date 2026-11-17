using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Books.GetById;

public sealed record GetBookByIdQuery(long Id) : IQuery<BookResponse>;

