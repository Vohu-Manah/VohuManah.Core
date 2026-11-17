using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Books;

namespace VohuManah.Application.Library.Books.GetAllEntities;

public sealed record GetAllBookEntitiesQuery : IQuery<List<Book>>;

