using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;
using VohuManah.Domain.Gaps;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Gaps.GetNames;

internal sealed class GetGapNamesQueryHandler(
    IApplicationDbContext dbContext) : IQueryHandler<GetGapNamesQuery, List<SelectItemResponse>>
{
    public Task<Result<List<SelectItemResponse>>> Handle(GetGapNamesQuery query, CancellationToken cancellationToken)
    {
        DbSet<Gap> gaps = dbContext.Gaps;

        List<SelectItemResponse> items = gaps
            .OrderBy(g => g.Title)
            .Select(g => new SelectItemResponse { Id = g.Id, Title = g.Title })
            .ToList();

        if (query.AddAllItemInFirstRow)
        {
            items.Insert(0, new SelectItemResponse { Id = 0, Title = "همه افتادگی ها" });
        }

        return Task.FromResult(Result.Success(items));
    }
}


