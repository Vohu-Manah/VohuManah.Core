using Application.Library._Shared;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Subjects.GetNames;

internal sealed class GetSubjectNamesQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetSubjectNamesQuery, List<SelectItemResponse>>
{
    public Task<Result<List<SelectItemResponse>>> Handle(GetSubjectNamesQuery query, CancellationToken cancellationToken)
    {
        DbSet<Subject> subjects = unitOfWork.Set<Subject>();

        List<SelectItemResponse> items = subjects
            .OrderBy(s => s.Title)
            .Select(s => new SelectItemResponse { Id = s.Id, Title = s.Title })
            .ToList();

        if (query.AddAllItemInFirstRow)
        {
            items.Insert(0, new SelectItemResponse { Id = 0, Title = "همه موضوعات" });
        }

        return Task.FromResult(Result.Success(items));
    }
}


