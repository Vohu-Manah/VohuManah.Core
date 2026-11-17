using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Publishers;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Publishers.GetList;

internal sealed class GetPublisherListQueryHandler(
    IApplicationDbContext dbContext,
    ICacheManager cacheManager) : IQueryHandler<GetPublisherListQuery, List<PublisherListResponse>>
{
    private const string PublisherAllKey = "sdp.publisher.all";

    public Task<Result<List<PublisherListResponse>>> Handle(GetPublisherListQuery query, CancellationToken cancellationToken)
    {
        var publishers = dbContext.Publishers;

        var allPublishers = cacheManager.Retrieve(PublisherAllKey, () => publishers.OrderBy(p => p.Name).ToList());

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

