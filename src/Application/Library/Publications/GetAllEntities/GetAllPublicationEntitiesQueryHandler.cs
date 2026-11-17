using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Library;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Publications.GetAllEntities;

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

