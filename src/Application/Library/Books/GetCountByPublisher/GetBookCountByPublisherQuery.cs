using Application.Abstractions.Messaging;
using Application.Library._Shared;

namespace Application.Library.Books.GetCountByPublisher;

public sealed record GetBookCountByPublisherQuery : IQuery<List<ListItemResponse>>;

