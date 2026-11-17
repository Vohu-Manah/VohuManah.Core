using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Subjects;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Subjects.Create;

internal sealed class CreateSubjectCommandHandler(
    IApplicationDbContext dbContext,
    ICacheManager cacheManager) : ICommandHandler<CreateSubjectCommand, int>
{
    private const string SubjectPatternKey = "sdp.subject.";

    public async Task<Result<int>> Handle(CreateSubjectCommand command, CancellationToken cancellationToken)
    {
        DbSet<Subject> subjects = dbContext.Subjects;

        if (await subjects.AnyAsync(s => s.Title == command.Title, cancellationToken))
        {
            return Result.Failure<int>(SubjectErrors.TitleNotUnique);
        }

        var subject = new Subject { Title = command.Title };
        subjects.Add(subject);
        await dbContext.SaveChangesAsync(cancellationToken);

        cacheManager.RemoveByPattern(SubjectPatternKey);

        return subject.Id;
    }
}


