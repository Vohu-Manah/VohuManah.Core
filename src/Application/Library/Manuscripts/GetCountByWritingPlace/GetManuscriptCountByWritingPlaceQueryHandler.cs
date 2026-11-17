using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;
using VohuManah.Domain.Library;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Manuscripts.GetCountByWritingPlace;

internal sealed class GetManuscriptCountByWritingPlaceQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetManuscriptCountByWritingPlaceQuery, List<ListItemResponse>>
{
    public async Task<Result<List<ListItemResponse>>> Handle(GetManuscriptCountByWritingPlaceQuery query, CancellationToken cancellationToken)
    {
        var manuscripts = unitOfWork.GetDbSet<Manuscript>();

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

