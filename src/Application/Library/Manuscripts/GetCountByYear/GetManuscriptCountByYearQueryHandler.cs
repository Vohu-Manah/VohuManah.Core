using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;
using VohuManah.Domain.Manuscripts;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Manuscripts.GetCountByYear;

internal sealed class GetManuscriptCountByYearQueryHandler(
    IApplicationDbContext dbContext) : IQueryHandler<GetManuscriptCountByYearQuery, List<ListItemResponse>>
{
    public async Task<Result<List<ListItemResponse>>> Handle(GetManuscriptCountByYearQuery query, CancellationToken cancellationToken)
    {
        var manuscripts = dbContext.Manuscripts;

        var list = await manuscripts
            .GroupBy(m => m.Year)
            .Select(g => new ListItemResponse
            {
                Text = g.Key,
                Value = g.Count()
            })
            .ToListAsync(cancellationToken);

        return Result.Success(list);
    }
}

