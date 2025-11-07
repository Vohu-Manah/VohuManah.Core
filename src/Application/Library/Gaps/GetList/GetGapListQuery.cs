using Application.Abstractions.Messaging;

namespace Application.Library.Gaps.GetList;

public sealed record GetGapListQuery : IQuery<List<GapListResponse>>;

