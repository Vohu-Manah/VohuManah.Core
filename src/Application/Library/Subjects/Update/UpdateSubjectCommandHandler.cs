using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Subjects;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Subjects.Update;

internal sealed class UpdateSubjectCommandHandler(
    IApplicationDbContext dbContext,
    ICacheManager cacheManager) : ICommandHandler<UpdateSubjectCommand>
{
    private const string SubjectPatternKey = "sdp.subject.";

    public async Task<Result> Handle(UpdateSubjectCommand command, CancellationToken cancellationToken)
    {
        DbSet<Subject> subjects = dbContext.Subjects;

        Subject? existing = await subjects.SingleOrDefaultAsync(s => s.Id == command.Id, cancellationToken);
        if (existing == null)
        {
            return Result.Failure(Error.NotFound("Subjects.NotFound", $"Subject with Id {command.Id} not found"));
        }

        if (await subjects.AnyAsync(s => s.Id != command.Id && s.Title == command.Title, cancellationToken))
        {
            return Result.Failure(SubjectErrors.TitleNotUnique);
        }

        existing.Title = command.Title;
        await dbContext.SaveChangesAsync(cancellationToken);

        cacheManager.RemoveByPattern(SubjectPatternKey);

        return Result.Success();
    }
}


