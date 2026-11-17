using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;
using VohuManah.Domain.Manuscripts;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Manuscripts.GetCountBySubject;

internal sealed class GetManuscriptCountBySubjectQueryHandler(
    IApplicationDbContext dbContext) : IQueryHandler<GetManuscriptCountBySubjectQuery, List<ListItemResponse>>
{
    public async Task<Result<List<ListItemResponse>>> Handle(GetManuscriptCountBySubjectQuery query, CancellationToken cancellationToken)
    {
        var manuscripts = dbContext.Manuscripts;

        var list = await manuscripts
            .Include(m => m.Subject)
            .GroupBy(m => new { m.SubjectId, SubjectTitle = m.Subject != null ? m.Subject.Title : "" })
            .Select(g => new ListItemResponse
            {
                Text = g.Key.SubjectTitle,
                Value = g.Count()
            })
            .ToListAsync(cancellationToken);

        return Result.Success(list);
    }
}

