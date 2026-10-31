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
        DbSet<Publication> publications = unitOfWork.GetDbSet<Publication>();

        List<Publication> allPublications = await publications.ToListAsync(cancellationToken);

        return Result.Success(allPublications);
    }
}

