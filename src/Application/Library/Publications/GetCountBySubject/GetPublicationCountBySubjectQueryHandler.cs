using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Library._Shared;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Publications.GetCountBySubject;

internal sealed class GetPublicationCountBySubjectQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetPublicationCountBySubjectQuery, List<ListItemResponse>>
{
    public async Task<Result<List<ListItemResponse>>> Handle(GetPublicationCountBySubjectQuery query, CancellationToken cancellationToken)
    {
        var publications = unitOfWork.Set<Publication>();
        
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

