using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;

namespace VohuManah.Application.Library.Publications.GetCountByPublishPlace;

public sealed record GetPublicationCountByPublishPlaceQuery : IQuery<List<ListItemResponse>>;

