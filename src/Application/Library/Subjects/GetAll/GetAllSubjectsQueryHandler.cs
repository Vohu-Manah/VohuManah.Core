using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Library;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Subjects.GetAll;

internal sealed class GetAllSubjectsQueryHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : IQueryHandler<GetAllSubjectsQuery, List<SubjectResponse>>
{
    private const string SubjectAllKey = "sdp.subject.all";

    public Task<Result<List<SubjectResponse>>> Handle(GetAllSubjectsQuery query, CancellationToken cancellationToken)
    {
        DbSet<Subject> subjects = unitOfWork.GetDbSet<Subject>();

        List<Subject> allSubjects = cacheManager.Retrieve(SubjectAllKey, () => subjects.ToList());

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


