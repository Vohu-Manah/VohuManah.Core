using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Gaps;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Gaps.Update;

internal sealed class UpdateGapCommandHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : ICommandHandler<UpdateGapCommand>
{
    private const string GapPatternKey = "sdp.gap.";

    public async Task<Result> Handle(UpdateGapCommand command, CancellationToken cancellationToken)
    {
        DbSet<Gap> gaps = unitOfWork.GetDbSet<Gap>();

        Gap? existing = await gaps
            .SingleOrDefaultAsync(g => g.Id == command.Id, cancellationToken);

        if (existing == null)
        {
            return Result.Failure(GapErrors.NotFound(command.Id));
        }

        if (await gaps.AnyAsync(g => g.Id != command.Id && g.Title == command.Title, cancellationToken))
        {
            return Result.Failure(GapErrors.TitleNotUnique);
        }

        existing.Title = command.Title;
        existing.Note = command.Note;
        existing.State = command.State;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        cacheManager.RemoveByPattern(GapPatternKey);

        return Result.Success();
    }
}

