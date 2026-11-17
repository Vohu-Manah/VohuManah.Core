using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Gaps;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Gaps.GetById;

internal sealed class GetGapByIdQueryHandler(
    IApplicationDbContext dbContext) : IQueryHandler<GetGapByIdQuery, GapResponse>
{
    public async Task<Result<GapResponse>> Handle(GetGapByIdQuery query, CancellationToken cancellationToken)
    {
        DbSet<Gap> gaps = dbContext.Gaps;

        Gap? gap = await gaps
            .SingleOrDefaultAsync(g => g.Id == query.Id, cancellationToken);

        if (gap == null)
        {
            return Result.Failure<GapResponse>(GapErrors.NotFound(query.Id));
        }

        GapResponse response = new GapResponse
        {
            Id = gap.Id,
            Title = gap.Title,
            Note = gap.Note,
            State = gap.State
        };

        return Result.Success(response);
    }
}

