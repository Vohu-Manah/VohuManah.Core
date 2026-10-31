using Application.Abstractions.Messaging;
using Application.Library.Shared;

namespace Application.Library.Manuscripts.GetCountByLanguage;

public sealed record GetManuscriptCountByLanguageQuery : IQuery<List<ListItemResponse>>;

