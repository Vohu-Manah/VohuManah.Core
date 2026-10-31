using Application.Abstractions.Messaging;
using Application.Library.Shared;

namespace Application.Library.Publications.GetCountByPublishDate;

public sealed record GetPublicationCountByPublishDateQuery : IQuery<List<ListItemResponse>>;

