using Application.Abstractions.Messaging;
using Domain.Library;

namespace Application.Library.Books.GetAllEntities;

public sealed record GetAllBookEntitiesQuery : IQuery<List<Book>>;

