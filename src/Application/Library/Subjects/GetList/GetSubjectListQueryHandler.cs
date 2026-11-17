using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Subjects;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Subjects.GetList;

internal sealed class GetSubjectListQueryHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : IQueryHandler<GetSubjectListQuery, List<SubjectListResponse>>
{
    private const string SubjectAllKey = "sdp.subject.all";

    public Task<Result<List<SubjectListResponse>>> Handle(GetSubjectListQuery query, CancellationToken cancellationToken)
    {
        var subjects = unitOfWork.GetDbSet<Subject>();

        var allSubjects = cacheManager.Retrieve(SubjectAllKey, () => subjects.ToList());

        var list = allSubjects.Select(s => new SubjectListResponse
        {
            Id = s.Id,
            Title = s.Title
        }).ToList();

        return Task.FromResult(Result.Success(list));
    }
}

