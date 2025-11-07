using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Gaps.GetById;

internal sealed class GetGapByIdQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetGapByIdQuery, GapResponse>
{
    public async Task<Result<GapResponse>> Handle(GetGapByIdQuery query, CancellationToken cancellationToken)
    {
        DbSet<Gap> gaps = unitOfWork.Set<Gap>();
        
        Gap? gap = await gaps
            .SingleOrDefaultAsync(g => g.Id == query.Id, cancellationToken);

        if (gap == null)
        {
            return Result.Failure<GapResponse>(GapErrors.NotFound(query.Id));
        }

        GapResponse response = new GapResponse {
            Id = gap.Id,
            Title = gap.Title,
            Note = gap.Note,
            State = gap.State
        };

        return Result.Success(response);
    }
}

