using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Manuscripts.GetCount;

internal sealed class GetManuscriptCountQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetManuscriptCountQuery, int>
{
    public async Task<Result<int>> Handle(GetManuscriptCountQuery query, CancellationToken cancellationToken)
    {
        var manuscripts = unitOfWork.Set<Manuscript>();
        
        int count = await manuscripts.CountAsync(cancellationToken);

        return Result.Success(count);
    }
}

