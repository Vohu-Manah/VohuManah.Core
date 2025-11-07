using Application.Library._Shared;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Gaps.GetNames;

internal sealed class GetGapNamesQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetGapNamesQuery, List<SelectItemResponse>>
{
    public Task<Result<List<SelectItemResponse>>> Handle(GetGapNamesQuery query, CancellationToken cancellationToken)
    {
        DbSet<Gap> gaps = unitOfWork.Set<Gap>();

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


