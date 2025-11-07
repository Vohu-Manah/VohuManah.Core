using Application.Abstractions.Messaging;

namespace Application.Library.Publishers.GetAll;

public sealed record GetAllPublishersQuery(bool AddAllItemInFirstRow = false) : IQuery<List<PublisherResponse>>;


