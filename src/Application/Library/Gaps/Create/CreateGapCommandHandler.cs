using Application.Abstractions.Caching;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Gaps.Create;

internal sealed class CreateGapCommandHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : ICommandHandler<CreateGapCommand, int>
{
    private const string GapPatternKey = "sdp.gap.";

    public async Task<Result<int>> Handle(CreateGapCommand command, CancellationToken cancellationToken)
    {
        DbSet<Gap> gaps = unitOfWork.Set<Gap>();
        
        if (await gaps.AnyAsync(g => g.Title == command.Title, cancellationToken))
        {
            return Result.Failure<int>(GapErrors.TitleNotUnique);
        }

        Gap gap = new Gap {
            Title = command.Title,
            Note = command.Note,
            State = command.State
        };

        gaps.Add(gap);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        cacheManager.RemoveByPattern(GapPatternKey);

        return gap.Id;
    }
}

