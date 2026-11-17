using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Library;

namespace VohuManah.Application.Library.Books.GetAllEntities;

public sealed record GetAllBookEntitiesQuery : IQuery<List<Book>>;

