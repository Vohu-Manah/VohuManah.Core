using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Caching;
using Domain.Library;
using SharedKernel;

namespace Application.Library.PublicationTypes.GetList;

internal sealed class GetPublicationTypeListQueryHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : IQueryHandler<GetPublicationTypeListQuery, List<PublicationTypeListResponse>>
{
    private const string PublicationTypeAllKey = "sdp.publicationtype.all";

    public Task<Result<List<PublicationTypeListResponse>>> Handle(GetPublicationTypeListQuery query, CancellationToken cancellationToken)
    {
        var publicationTypes = unitOfWork.Set<PublicationType>();
        
        var allPublicationTypes = cacheManager.Get(PublicationTypeAllKey, () => publicationTypes.ToList());
        
        var list = allPublicationTypes.Select(pt => new PublicationTypeListResponse
        {
            Id = pt.Id,
            Title = pt.Title
        }).ToList();

        return Task.FromResult(Result.Success(list));
    }
}

