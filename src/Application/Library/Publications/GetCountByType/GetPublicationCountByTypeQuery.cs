using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;

namespace VohuManah.Application.Library.Publications.GetCountByType;

public sealed record GetPublicationCountByTypeQuery : IQuery<List<ListItemResponse>>;

