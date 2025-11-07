using Application.Abstractions.Messaging;

namespace Application.Library.Publishers.GetList;

public sealed record GetPublisherListQuery : IQuery<List<PublisherListResponse>>;

