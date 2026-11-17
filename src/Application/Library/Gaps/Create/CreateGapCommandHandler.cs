using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Gaps;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Gaps.Create;

internal sealed class CreateGapCommandHandler(
    IApplicationDbContext dbContext,
    ICacheManager cacheManager) : ICommandHandler<CreateGapCommand, int>
{
    private const string GapPatternKey = "sdp.gap.";

    public async Task<Result<int>> Handle(CreateGapCommand command, CancellationToken cancellationToken)
    {
        DbSet<Gap> gaps = dbContext.Gaps;

        if (await gaps.AnyAsync(g => g.Title == command.Title, cancellationToken))
        {
            return Result.Failure<int>(GapErrors.TitleNotUnique);
        }

        var gap = new Gap
        {
            Title = command.Title,
            Note = command.Note,
            State = command.State
        };

        gaps.Add(gap);
        await dbContext.SaveChangesAsync(cancellationToken);

        cacheManager.RemoveByPattern(GapPatternKey);

        return gap.Id;
    }
}

