using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Manuscripts;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Manuscripts.GetCount;

internal sealed class GetManuscriptCountQueryHandler(
    IApplicationDbContext dbContext) : IQueryHandler<GetManuscriptCountQuery, int>
{
    public async Task<Result<int>> Handle(GetManuscriptCountQuery query, CancellationToken cancellationToken)
    {
        var manuscripts = dbContext.Manuscripts;

        int count = await manuscripts.CountAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success(count);
    }
}

