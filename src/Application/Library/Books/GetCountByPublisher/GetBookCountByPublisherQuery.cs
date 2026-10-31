using Application.Abstractions.Messaging;
using Application.Library.Shared;

namespace Application.Library.Books.GetCountByPublisher;

public sealed record GetBookCountByPublisherQuery : IQuery<List<ListItemResponse>>;

