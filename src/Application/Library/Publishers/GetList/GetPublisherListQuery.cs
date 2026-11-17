using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Publishers.GetList;

public sealed record GetPublisherListQuery : IQuery<List<PublisherListResponse>>;

