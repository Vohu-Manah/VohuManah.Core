using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Manuscripts.GetAllEntities;

internal sealed class GetAllManuscriptEntitiesQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetAllManuscriptEntitiesQuery, List<Manuscript>>
{
    public async Task<Result<List<Manuscript>>> Handle(GetAllManuscriptEntitiesQuery query, CancellationToken cancellationToken)
    {
        var manuscripts = unitOfWork.Set<Manuscript>();
        
        var allManuscripts = await manuscripts.ToListAsync(cancellationToken);

        return Result.Success(allManuscripts);
    }
}

