using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Library;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Subjects.GetById;

internal sealed class GetSubjectByIdQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetSubjectByIdQuery, SubjectResponse>
{
    public async Task<Result<SubjectResponse>> Handle(GetSubjectByIdQuery query, CancellationToken cancellationToken)
    {
        DbSet<Subject> subjects = unitOfWork.GetDbSet<Subject>();

        Subject? subject = await subjects
            .SingleOrDefaultAsync(s => s.Id == query.Id, cancellationToken);

        if (subject == null)
        {
            return Result.Failure<SubjectResponse>(SubjectErrors.NotFound(query.Id));
        }

        SubjectResponse response = new SubjectResponse
        {
            Id = subject.Id,
            Title = subject.Title
        };

        return Result.Success(response);
    }
}

