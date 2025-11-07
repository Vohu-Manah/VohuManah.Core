using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Application.Abstractions.Caching;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Subjects.GetAll;

internal sealed class GetAllSubjectsQueryHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : IQueryHandler<GetAllSubjectsQuery, List<SubjectResponse>>
{
    private const string SubjectAllKey = "sdp.subject.all";

    public Task<Result<List<SubjectResponse>>> Handle(GetAllSubjectsQuery query, CancellationToken cancellationToken)
    {
        DbSet<Subject> subjects = unitOfWork.Set<Subject>();
        
        List<Subject> allSubjects = cacheManager.Get(SubjectAllKey, () => subjects.ToList());
        
        List<SubjectResponse> response = allSubjects.Select(s => new SubjectResponse
        {
            Id = s.Id,
            Title = s.Title
        }).ToList();

        if (query.AddAllItemInFirstRow)
        {
            response.Insert(0, new SubjectResponse { Id = 0, Title = "همه موضوعات" });
        }

        return Task.FromResult(Result.Success(response));
    }
}


