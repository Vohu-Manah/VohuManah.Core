using Application.Abstractions.Messaging;
using Application.Library._Shared;

namespace Application.Library.Books.GetCountByLanguage;

public sealed record GetBookCountByLanguageQuery : IQuery<List<ListItemResponse>>;

