using Application.Abstractions.Messaging;
using Application.Library.Shared;

namespace Application.Library.Books.GetCountByLanguage;

public sealed record GetBookCountByLanguageQuery : IQuery<List<ListItemResponse>>;

