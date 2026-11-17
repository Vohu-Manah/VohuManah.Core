using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Library;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Subjects.Create;

internal sealed class CreateSubjectCommandHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : ICommandHandler<CreateSubjectCommand, int>
{
    private const string SubjectPatternKey = "sdp.subject.";

    public async Task<Result<int>> Handle(CreateSubjectCommand command, CancellationToken cancellationToken)
    {
        DbSet<Subject> subjects = unitOfWork.GetDbSet<Subject>();

        if (await subjects.AnyAsync(s => s.Title == command.Title, cancellationToken))
        {
            return Result.Failure<int>(SubjectErrors.TitleNotUnique);
        }

        var subject = new Subject { Title = command.Title };
        subjects.Add(subject);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        cacheManager.RemoveByPattern(SubjectPatternKey);

        return subject.Id;
    }
}


