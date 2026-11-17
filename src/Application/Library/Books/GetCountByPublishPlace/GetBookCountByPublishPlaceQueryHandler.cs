using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;
using VohuManah.Domain.Books;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Books.GetCountByPublishPlace;

internal sealed class GetBookCountByPublishPlaceQueryHandler(
    IApplicationDbContext dbContext) : IQueryHandler<GetBookCountByPublishPlaceQuery, List<ListItemResponse>>
{
    public async Task<Result<List<ListItemResponse>>> Handle(GetBookCountByPublishPlaceQuery query, CancellationToken cancellationToken)
    {
        var books = dbContext.Books;

        var list = await books
            .Include(b => b.City)
            .GroupBy(b => new { b.PublishPlaceId, CityName = b.City != null ? b.City.Name : "" })
            .Select(g => new ListItemResponse
            {
                Text = g.Key.CityName,
                Value = g.Count()
            })
            .ToListAsync(cancellationToken);

        return Result.Success(list);
    }
}

