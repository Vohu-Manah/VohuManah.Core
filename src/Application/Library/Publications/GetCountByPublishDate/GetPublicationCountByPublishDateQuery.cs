using Application.Abstractions.Messaging;
using Application.Library._Shared;

namespace Application.Library.Publications.GetCountByPublishDate;

public sealed record GetPublicationCountByPublishDateQuery : IQuery<List<ListItemResponse>>;

