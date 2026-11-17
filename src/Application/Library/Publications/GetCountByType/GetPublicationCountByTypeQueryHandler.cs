using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;
using VohuManah.Domain.Publications;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Publications.GetCountByType;

internal sealed class GetPublicationCountByTypeQueryHandler(
    IApplicationDbContext dbContext) : IQueryHandler<GetPublicationCountByTypeQuery, List<ListItemResponse>>
{
    public async Task<Result<List<ListItemResponse>>> Handle(GetPublicationCountByTypeQuery query, CancellationToken cancellationToken)
    {
        var publications = dbContext.Publications;

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

