using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Gaps;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Gaps.GetList;

internal sealed class GetGapListQueryHandler(
    IApplicationDbContext dbContext,
    ICacheManager cacheManager) : IQueryHandler<GetGapListQuery, List<GapListResponse>>
{
    private const string GapAllKey = "sdp.gap.all";

    public Task<Result<List<GapListResponse>>> Handle(GetGapListQuery query, CancellationToken cancellationToken)
    {
        var gaps = dbContext.Gaps;

        var allGaps = cacheManager.Retrieve(GapAllKey, () => gaps.OrderByDescending(g => g.Id).ToList());

        var list = allGaps.Select(g => new GapListResponse
        {
            Id = g.Id,
            Title = g.Title,
            Note = g.Note,
            StateTitle = g.State ? "دارد" : "ندارد"
        }).ToList();

        return Task.FromResult(Result.Success(list));
    }
}

