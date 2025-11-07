using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Caching;
using Domain.Library;
using SharedKernel;

namespace Application.Library.Publishers.GetList;

internal sealed class GetPublisherListQueryHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : IQueryHandler<GetPublisherListQuery, List<PublisherListResponse>>
{
    private const string PublisherAllKey = "sdp.publisher.all";

    public Task<Result<List<PublisherListResponse>>> Handle(GetPublisherListQuery query, CancellationToken cancellationToken)
    {
        var publishers = unitOfWork.Set<Publisher>();
        
        var allPublishers = cacheManager.Get(PublisherAllKey, () => publishers.OrderBy(p => p.Name).ToList());
        
        var list = allPublishers.Select(p => new PublisherListResponse
        {
            Id = p.Id,
            Name = p.Name,
            ManagerName = p.ManagerName,
            Telephone = p.Telephone,
            Address = p.Address
        }).ToList();

        return Task.FromResult(Result.Success(list));
    }
}

