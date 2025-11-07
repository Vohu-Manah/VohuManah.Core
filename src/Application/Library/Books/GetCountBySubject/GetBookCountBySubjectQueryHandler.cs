using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Library._Shared;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Books.GetCountBySubject;

internal sealed class GetBookCountBySubjectQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetBookCountBySubjectQuery, List<ListItemResponse>>
{
    public async Task<Result<List<ListItemResponse>>> Handle(GetBookCountBySubjectQuery query, CancellationToken cancellationToken)
    {
        var books = unitOfWork.Set<Book>();
        
        var list = await books
            .Include(b => b.Subject)
            .GroupBy(b => new { b.SubjectId, SubjectTitle = b.Subject != null ? b.Subject.Title : "" })
            .Select(g => new ListItemResponse
            {
                Text = g.Key.SubjectTitle,
                Value = g.Count()
            })
            .ToListAsync(cancellationToken);

        return Result.Success(list);
    }
}

