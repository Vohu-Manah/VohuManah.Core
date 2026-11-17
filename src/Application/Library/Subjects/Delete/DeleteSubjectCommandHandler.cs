using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Library;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Subjects.Delete;

internal sealed class DeleteSubjectCommandHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : ICommandHandler<DeleteSubjectCommand>
{
    private const string SubjectPatternKey = "sdp.subject.";

    public async Task<Result> Handle(DeleteSubjectCommand command, CancellationToken cancellationToken)
    {
        DbSet<Subject> subjects = unitOfWork.GetDbSet<Subject>();

        Subject? existing = await subjects.SingleOrDefaultAsync(s => s.Id == command.Id, cancellationToken);
        if (existing == null)
        {
            return Result.Failure(Error.NotFound("Subjects.NotFound", $"Subject with Id {command.Id} not found"));
        }

        subjects.Remove(existing);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        cacheManager.RemoveByPattern(SubjectPatternKey);

        return Result.Success();
    }
}


