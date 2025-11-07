using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Library._Shared;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Books.GetCountByYear;

internal sealed class GetBookCountByYearQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetBookCountByYearQuery, List<ListItemResponse>>
{
    public async Task<Result<List<ListItemResponse>>> Handle(GetBookCountByYearQuery query, CancellationToken cancellationToken)
    {
        var books = unitOfWork.Set<Book>();
        
        var list = await books
            .GroupBy(b => b.PublishYear)
            .Select(g => new ListItemResponse
            {
                Text = g.Key,
                Value = g.Count()
            })
            .ToListAsync(cancellationToken);

        return Result.Success(list);
    }
}

