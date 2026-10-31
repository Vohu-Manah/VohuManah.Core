using Application.Abstractions.Messaging;
using Application.Library.Shared;

namespace Application.Library.Publications.GetCountByType;

public sealed record GetPublicationCountByTypeQuery : IQuery<List<ListItemResponse>>;

