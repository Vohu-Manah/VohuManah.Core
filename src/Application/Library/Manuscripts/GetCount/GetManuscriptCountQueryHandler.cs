using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Library;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Manuscripts.GetCount;

internal sealed class GetManuscriptCountQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetManuscriptCountQuery, int>
{
    public async Task<Result<int>> Handle(GetManuscriptCountQuery query, CancellationToken cancellationToken)
    {
        var manuscripts = unitOfWork.GetDbSet<Manuscript>();

        int count = await manuscripts.CountAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success(count);
    }
}

