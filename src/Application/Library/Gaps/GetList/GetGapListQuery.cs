using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Gaps.GetList;

public sealed record GetGapListQuery : IQuery<List<GapListResponse>>;

