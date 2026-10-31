using Application.Abstractions.Messaging;
using Application.Library.Shared;

namespace Application.Library.Publications.GetCountByPublishPlace;

public sealed record GetPublicationCountByPublishPlaceQuery : IQuery<List<ListItemResponse>>;

