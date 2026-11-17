using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;
using VohuManah.Domain.Publications;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Publications.GetCountBySubject;

internal sealed class GetPublicationCountBySubjectQueryHandler(
    IApplicationDbContext dbContext) : IQueryHandler<GetPublicationCountBySubjectQuery, List<ListItemResponse>>
{
    public async Task<Result<List<ListItemResponse>>> Handle(GetPublicationCountBySubjectQuery query, CancellationToken cancellationToken)
    {
        var publications = dbContext.Publications;

        var list = await publications
            .Include(p => p.Subject)
            .GroupBy(p => new { p.SubjectId, SubjectTitle = p.Subject != null ? p.Subject.Title : "" })
            .Select(g => new ListItemResponse
            {
                Text = g.Key.SubjectTitle,
                Value = g.Count()
            })
            .ToListAsync(cancellationToken);

        return Result.Success(list);
    }
}

