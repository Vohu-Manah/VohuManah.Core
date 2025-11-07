using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Application.Abstractions.Caching;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Subjects.Update;

internal sealed class UpdateSubjectCommandHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : ICommandHandler<UpdateSubjectCommand>
{
    private const string SubjectPatternKey = "sdp.subject.";

    public async Task<Result> Handle(UpdateSubjectCommand command, CancellationToken cancellationToken)
    {
        DbSet<Subject> subjects = unitOfWork.Set<Subject>();
        
        Subject? existing = await subjects.SingleOrDefaultAsync(s => s.Id == command.Id, cancellationToken);
        if (existing == null)
        {
            return Result.Failure(Error.NotFound("Subjects.NotFound", $"Subject with Id {command.Id} not found"));
        }

        if (await subjects.AnyAsync(s => s.Id != command.Id && s.Title == command.Title, cancellationToken))
        {
            return Result.Failure(Error.Conflict("Subjects.TitleNotUnique", "A subject with this title already exists"));
        }

        existing.Title = command.Title;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        cacheManager.RemoveByPattern(SubjectPatternKey);

        return Result.Success();
    }
}


