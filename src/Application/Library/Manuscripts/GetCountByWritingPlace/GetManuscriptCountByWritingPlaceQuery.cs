using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;

namespace VohuManah.Application.Library.Manuscripts.GetCountByWritingPlace;

public sealed record GetManuscriptCountByWritingPlaceQuery : IQuery<List<ListItemResponse>>;

