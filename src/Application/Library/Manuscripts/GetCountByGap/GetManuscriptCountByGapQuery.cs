using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;

namespace VohuManah.Application.Library.Manuscripts.GetCountByGap;

public sealed record GetManuscriptCountByGapQuery : IQuery<List<ListItemResponse>>;

