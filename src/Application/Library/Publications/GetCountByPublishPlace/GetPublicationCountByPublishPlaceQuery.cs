using Application.Abstractions.Messaging;
using Application.Library._Shared;

namespace Application.Library.Publications.GetCountByPublishPlace;

public sealed record GetPublicationCountByPublishPlaceQuery : IQuery<List<ListItemResponse>>;

