using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Library._Shared;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Manuscripts.GetCountBySubject;

internal sealed class GetManuscriptCountBySubjectQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetManuscriptCountBySubjectQuery, List<ListItemResponse>>
{
    public async Task<Result<List<ListItemResponse>>> Handle(GetManuscriptCountBySubjectQuery query, CancellationToken cancellationToken)
    {
        var manuscripts = unitOfWork.Set<Manuscript>();
        
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

