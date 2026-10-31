using Application.Abstractions.Caching;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using SharedKernel;

namespace Application.Library.Gaps.GetList;

internal sealed class GetGapListQueryHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : IQueryHandler<GetGapListQuery, List<GapListResponse>>
{
    private const string GapAllKey = "sdp.gap.all";

    public Task<Result<List<GapListResponse>>> Handle(GetGapListQuery query, CancellationToken cancellationToken)
    {
        var gaps = unitOfWork.GetDbSet<Gap>();

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

