using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;
using VohuManah.Domain.Library;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Publications.GetCountByType;

internal sealed class GetPublicationCountByTypeQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetPublicationCountByTypeQuery, List<ListItemResponse>>
{
    public async Task<Result<List<ListItemResponse>>> Handle(GetPublicationCountByTypeQuery query, CancellationToken cancellationToken)
    {
        var publications = unitOfWork.GetDbSet<Publication>();

        var list = await publications
            .Include(p => p.PublicationType)
            .GroupBy(p => new { p.TypeId, TypeTitle = p.PublicationType != null ? p.PublicationType.Title : "" })
            .Select(g => new ListItemResponse
            {
                Text = g.Key.TypeTitle,
                Value = g.Count()
            })
            .ToListAsync(cancellationToken);

        return Result.Success(list);
    }
}

