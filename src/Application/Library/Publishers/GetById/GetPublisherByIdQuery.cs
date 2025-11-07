using Application.Abstractions.Messaging;

namespace Application.Library.Publishers.GetById;

public sealed record GetPublisherByIdQuery(int Id) : IQuery<PublisherResponse>;

