using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Library._Shared;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Books.GetCountByPublishPlace;

internal sealed class GetBookCountByPublishPlaceQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetBookCountByPublishPlaceQuery, List<ListItemResponse>>
{
    public async Task<Result<List<ListItemResponse>>> Handle(GetBookCountByPublishPlaceQuery query, CancellationToken cancellationToken)
    {
        var books = unitOfWork.Set<Book>();
        
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

