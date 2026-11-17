using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;
using VohuManah.Domain.Manuscripts;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Manuscripts.GetCountByGap;

internal sealed class GetManuscriptCountByGapQueryHandler(
    IApplicationDbContext dbContext) : IQueryHandler<GetManuscriptCountByGapQuery, List<ListItemResponse>>
{
    public async Task<Result<List<ListItemResponse>>> Handle(GetManuscriptCountByGapQuery query, CancellationToken cancellationToken)
    {
        var manuscripts = dbContext.Manuscripts;

        var list = await manuscripts
            .Include(m => m.Gap)
            .GroupBy(m => m.Gap != null && m.Gap.State)
            .Select(g => new ListItemResponse
            {
                Text = g.Key ? "دارد" : "ندارد",
                Value = g.Count()
            })
            .ToListAsync(cancellationToken);

        return Result.Success(list);
    }
}

