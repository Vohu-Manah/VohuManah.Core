using Application.Abstractions.Messaging;
using Application.Library._Shared;

namespace Application.Library.Books.GetCountByPublishPlace;

public sealed record GetBookCountByPublishPlaceQuery : IQuery<List<ListItemResponse>>;

