using Application.Abstractions.Caching;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Gaps.Delete;

internal sealed class DeleteGapCommandHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : ICommandHandler<DeleteGapCommand>
{
    private const string GapPatternKey = "sdp.gap.";

    public async Task<Result> Handle(DeleteGapCommand command, CancellationToken cancellationToken)
    {
        DbSet<Gap> gaps = unitOfWork.Set<Gap>();
        
        Gap? gap = await gaps
            .SingleOrDefaultAsync(g => g.Id == command.Id, cancellationToken);

        if (gap == null)
        {
            return Result.Failure(GapErrors.NotFound(command.Id));
        }

        gaps.Remove(gap);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        cacheManager.RemoveByPattern(GapPatternKey);

        return Result.Success();
    }
}

