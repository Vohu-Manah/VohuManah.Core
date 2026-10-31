using Application.Abstractions.Messaging;
using Application.Library.Shared;

namespace Application.Library.Books.GetCountByPublishPlace;

public sealed record GetBookCountByPublishPlaceQuery : IQuery<List<ListItemResponse>>;

