using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;

namespace VohuManah.Application.Library.Books.GetCountByPublishPlace;

public sealed record GetBookCountByPublishPlaceQuery : IQuery<List<ListItemResponse>>;

