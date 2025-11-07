using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Library._Shared;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Manuscripts.GetCountByWritingPlace;

internal sealed class GetManuscriptCountByWritingPlaceQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetManuscriptCountByWritingPlaceQuery, List<ListItemResponse>>
{
    public async Task<Result<List<ListItemResponse>>> Handle(GetManuscriptCountByWritingPlaceQuery query, CancellationToken cancellationToken)
    {
        var manuscripts = unitOfWork.Set<Manuscript>();
        
        var list = await manuscripts
            .Include(m => m.City)
            .GroupBy(m => new { m.WritingPlaceId, CityName = m.City != null ? m.City.Name : "" })
            .Select(g => new ListItemResponse
            {
                Text = g.Key.CityName,
                Value = g.Count()
            })
            .ToListAsync(cancellationToken);

        return Result.Success(list);
    }
}

