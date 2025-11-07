using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Library._Shared;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Manuscripts.GetCountByYear;

internal sealed class GetManuscriptCountByYearQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetManuscriptCountByYearQuery, List<ListItemResponse>>
{
    public async Task<Result<List<ListItemResponse>>> Handle(GetManuscriptCountByYearQuery query, CancellationToken cancellationToken)
    {
        var manuscripts = unitOfWork.Set<Manuscript>();
        
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

