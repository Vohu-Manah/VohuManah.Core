using Application.Abstractions.Messaging;
using Application.Library._Shared;

namespace Application.Library.Publications.GetCountByType;

public sealed record GetPublicationCountByTypeQuery : IQuery<List<ListItemResponse>>;

