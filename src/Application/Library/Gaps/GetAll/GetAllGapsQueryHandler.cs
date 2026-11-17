using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Gaps;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Gaps.GetAll;

internal sealed class GetAllGapsQueryHandler(
    IApplicationDbContext dbContext,
    ICacheManager cacheManager) : IQueryHandler<GetAllGapsQuery, List<GapResponse>>
{
    private const string GapAllKey = "sdp.gap.all";

    public Task<Result<List<GapResponse>>> Handle(GetAllGapsQuery query, CancellationToken cancellationToken)
    {
        DbSet<Gap> gaps = dbContext.Gaps;

        List<Gap> allGaps = cacheManager.Retrieve(GapAllKey, () => gaps.OrderByDescending(g => g.Id).ToList());

        List<GapResponse> response = allGaps.Select(g => new GapResponse
        {
            Id = g.Id,
            Title = g.Title,
            Note = g.Note,
            State = g.State
        }).ToList();

        return Task.FromResult(Result.Success(response));
    }
}

