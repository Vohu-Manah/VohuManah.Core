using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Publications.GetCount;

internal sealed class GetPublicationCountQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetPublicationCountQuery, int>
{
    public async Task<Result<int>> Handle(GetPublicationCountQuery query, CancellationToken cancellationToken)
    {
        var publications = unitOfWork.Set<Publication>();
        
        int count = await publications.CountAsync(cancellationToken);

        return Result.Success(count);
    }
}

