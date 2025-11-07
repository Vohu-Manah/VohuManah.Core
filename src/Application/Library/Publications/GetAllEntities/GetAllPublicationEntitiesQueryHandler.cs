using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Publications.GetAllEntities;

internal sealed class GetAllPublicationEntitiesQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetAllPublicationEntitiesQuery, List<Publication>>
{
    public async Task<Result<List<Publication>>> Handle(GetAllPublicationEntitiesQuery query, CancellationToken cancellationToken)
    {
        var publications = unitOfWork.Set<Publication>();
        
        var allPublications = await publications.ToListAsync(cancellationToken);

        return Result.Success(allPublications);
    }
}

