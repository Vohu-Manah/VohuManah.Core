using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;

namespace VohuManah.Application.Library.Publications.GetCountByPublishDate;

public sealed record GetPublicationCountByPublishDateQuery : IQuery<List<ListItemResponse>>;

