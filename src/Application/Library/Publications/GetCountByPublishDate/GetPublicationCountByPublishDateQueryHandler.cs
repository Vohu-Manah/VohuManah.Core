using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;
using VohuManah.Domain.Publications;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Publications.GetCountByPublishDate;

internal sealed class GetPublicationCountByPublishDateQueryHandler(
    IApplicationDbContext dbContext) : IQueryHandler<GetPublicationCountByPublishDateQuery, List<ListItemResponse>>
{
    public async Task<Result<List<ListItemResponse>>> Handle(GetPublicationCountByPublishDateQuery query, CancellationToken cancellationToken)
    {
        var publications = dbContext.Publications;

        var list = await publications
            .GroupBy(p => p.PublishDate)
            .Select(g => new ListItemResponse
            {
                Text = g.Key,
                Value = g.Count()
            })
            .ToListAsync(cancellationToken);

        return Result.Success(list);
    }
}

