using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Application.Abstractions.Caching;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Publishers.GetAll;

internal sealed class GetAllPublishersQueryHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : IQueryHandler<GetAllPublishersQuery, List<PublisherResponse>>
{
    private const string PublisherAllKey = "sdp.publisher.all";

    public Task<Result<List<PublisherResponse>>> Handle(GetAllPublishersQuery query, CancellationToken cancellationToken)
    {
        DbSet<Publisher> publishers = unitOfWork.Set<Publisher>();
        
        List<Publisher> allPublishers = cacheManager.Get(PublisherAllKey, () => publishers.OrderBy(p => p.Name).ToList());
        
        List<PublisherResponse> response = allPublishers.Select(p => new PublisherResponse
        {
            Id = p.Id,
            Name = p.Name,
            ManagerName = p.ManagerName,
            Address = p.Address,
            Telephone = p.Telephone
        }).ToList();

        if (query.AddAllItemInFirstRow)
        {
            response.Insert(0, new PublisherResponse { Id = 0, Name = "همه ناشران", ManagerName = "", Telephone = "", Address = "" });
        }

        return Task.FromResult(Result.Success(response));
    }
}


