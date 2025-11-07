using Application.Abstractions.Messaging;
using Application.Library._Shared;

namespace Application.Library.Publications.GetCountByLanguage;

public sealed record GetPublicationCountByLanguageQuery : IQuery<List<ListItemResponse>>;

