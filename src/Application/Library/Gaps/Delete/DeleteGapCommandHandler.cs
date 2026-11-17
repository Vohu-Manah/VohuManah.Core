using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Gaps;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Gaps.Delete;

internal sealed class DeleteGapCommandHandler(
    IApplicationDbContext dbContext,
    ICacheManager cacheManager) : ICommandHandler<DeleteGapCommand>
{
    private const string GapPatternKey = "sdp.gap.";

    public async Task<Result> Handle(DeleteGapCommand command, CancellationToken cancellationToken)
    {
        DbSet<Gap> gaps = dbContext.Gaps;

        Gap? gap = await gaps
            .SingleOrDefaultAsync(g => g.Id == command.Id, cancellationToken);

        if (gap == null)
        {
            return Result.Failure(GapErrors.NotFound(command.Id));
        }

        gaps.Remove(gap);
        await dbContext.SaveChangesAsync(cancellationToken);

        cacheManager.RemoveByPattern(GapPatternKey);

        return Result.Success();
    }
}

