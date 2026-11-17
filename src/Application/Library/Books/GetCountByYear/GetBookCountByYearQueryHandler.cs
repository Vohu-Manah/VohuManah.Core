using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;
using VohuManah.Domain.Books;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Books.GetCountByYear;

internal sealed class GetBookCountByYearQueryHandler(
    IApplicationDbContext dbContext) : IQueryHandler<GetBookCountByYearQuery, List<ListItemResponse>>
{
    public async Task<Result<List<ListItemResponse>>> Handle(GetBookCountByYearQuery query, CancellationToken cancellationToken)
    {
        var books = dbContext.Books;

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

