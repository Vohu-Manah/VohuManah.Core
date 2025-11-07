using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Application.Abstractions.Caching;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Subjects.Create;

internal sealed class CreateSubjectCommandHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : ICommandHandler<CreateSubjectCommand, int>
{
    private const string SubjectPatternKey = "sdp.subject.";

    public async Task<Result<int>> Handle(CreateSubjectCommand command, CancellationToken cancellationToken)
    {
        DbSet<Subject> subjects = unitOfWork.Set<Subject>();
        
        if (await subjects.AnyAsync(s => s.Title == command.Title, cancellationToken))
        {
            return Result.Failure<int>(Error.Conflict("Subjects.TitleNotUnique", "A subject with this title already exists"));
        }

        Subject subject = new Subject { Title = command.Title };
        subjects.Add(subject);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        cacheManager.RemoveByPattern(SubjectPatternKey);

        return subject.Id;
    }
}


