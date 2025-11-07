using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Subjects.GetById;

internal sealed class GetSubjectByIdQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetSubjectByIdQuery, SubjectResponse>
{
    public async Task<Result<SubjectResponse>> Handle(GetSubjectByIdQuery query, CancellationToken cancellationToken)
    {
        DbSet<Subject> subjects = unitOfWork.Set<Subject>();
        
        Subject? subject = await subjects
            .SingleOrDefaultAsync(s => s.Id == query.Id, cancellationToken);

        if (subject == null)
        {
            return Result.Failure<SubjectResponse>(SubjectErrors.NotFound(query.Id));
        }

        SubjectResponse response = new SubjectResponse {
            Id = subject.Id,
            Title = subject.Title
        };

        return Result.Success(response);
    }
}

