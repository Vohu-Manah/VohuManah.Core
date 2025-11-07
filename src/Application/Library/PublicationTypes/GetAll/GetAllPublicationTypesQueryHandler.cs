using Application.Abstractions.Caching;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.PublicationTypes.GetAll;

internal sealed class GetAllPublicationTypesQueryHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : IQueryHandler<GetAllPublicationTypesQuery, List<PublicationTypeResponse>>
{
    private const string PublicationTypeAllKey = "sdp.publicationtype.all";

    public Task<Result<List<PublicationTypeResponse>>> Handle(GetAllPublicationTypesQuery query, CancellationToken cancellationToken)
    {
        DbSet<PublicationType> publicationTypes = unitOfWork.Set<PublicationType>();
        
        List<PublicationType> allPublicationTypes = cacheManager.Get(PublicationTypeAllKey, () => publicationTypes.ToList());
        
        List<PublicationTypeResponse> response = allPublicationTypes.Select(pt => new PublicationTypeResponse
        {
            Id = pt.Id,
            Title = pt.Title
        }).ToList();

        return Task.FromResult(Result.Success(response));
    }
}

