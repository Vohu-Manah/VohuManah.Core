using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Library._Shared;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Publications.GetCountByPublishPlace;

internal sealed class GetPublicationCountByPublishPlaceQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetPublicationCountByPublishPlaceQuery, List<ListItemResponse>>
{
    public async Task<Result<List<ListItemResponse>>> Handle(GetPublicationCountByPublishPlaceQuery query, CancellationToken cancellationToken)
    {
        var publications = unitOfWork.Set<Publication>();
        
        var list = await publications
            .Include(p => p.City)
            .GroupBy(p => new { p.PublishPlaceId, CityName = p.City != null ? p.City.Name : "" })
            .Select(g => new ListItemResponse
            {
                Text = g.Key.CityName,
                Value = g.Count()
            })
            .ToListAsync(cancellationToken);

        return Result.Success(list);
    }
}

