using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;
using VohuManah.Domain.Library;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Publications.GetCountByPublishPlace;

internal sealed class GetPublicationCountByPublishPlaceQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetPublicationCountByPublishPlaceQuery, List<ListItemResponse>>
{
    public async Task<Result<List<ListItemResponse>>> Handle(GetPublicationCountByPublishPlaceQuery query, CancellationToken cancellationToken)
    {
        var publications = unitOfWork.GetDbSet<Publication>();

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

