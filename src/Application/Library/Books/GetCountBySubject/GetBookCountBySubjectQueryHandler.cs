using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;
using VohuManah.Domain.Library;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Books.GetCountBySubject;

internal sealed class GetBookCountBySubjectQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetBookCountBySubjectQuery, List<ListItemResponse>>
{
    public async Task<Result<List<ListItemResponse>>> Handle(GetBookCountBySubjectQuery query, CancellationToken cancellationToken)
    {
        var books = unitOfWork.GetDbSet<Book>();

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

