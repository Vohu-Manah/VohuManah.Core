using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Library._Shared;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Manuscripts.GetCountByGap;

internal sealed class GetManuscriptCountByGapQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetManuscriptCountByGapQuery, List<ListItemResponse>>
{
    public async Task<Result<List<ListItemResponse>>> Handle(GetManuscriptCountByGapQuery query, CancellationToken cancellationToken)
    {
        var manuscripts = unitOfWork.Set<Manuscript>();
        
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

