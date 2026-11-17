using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Library;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Publications.GetCount;

internal sealed class GetPublicationCountQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetPublicationCountQuery, int>
{
    public async Task<Result<int>> Handle(GetPublicationCountQuery query, CancellationToken cancellationToken)
    {
        var publications = unitOfWork.GetDbSet<Publication>();

        int count = await publications.CountAsync(cancellationToken);

        return Result.Success(count);
    }
}

