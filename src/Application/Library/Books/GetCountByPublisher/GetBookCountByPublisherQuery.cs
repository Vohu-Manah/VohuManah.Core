using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;

namespace VohuManah.Application.Library.Books.GetCountByPublisher;

public sealed record GetBookCountByPublisherQuery : IQuery<List<ListItemResponse>>;

