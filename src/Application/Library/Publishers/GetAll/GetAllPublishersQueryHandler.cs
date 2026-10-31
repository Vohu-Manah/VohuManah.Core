using Application.Abstractions.Caching;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
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
        DbSet<Publisher> publishers = unitOfWork.GetDbSet<Publisher>();

        List<Publisher> allPublishers = cacheManager.Retrieve(PublisherAllKey, () => publishers.OrderBy(p => p.Name).ToList());

        List<PublisherResponse> response = allPublishers.Select(p => new PublisherResponse
        {
            Id = p.Id,
            Name = p.Name,
            ManagerName = p.ManagerName,
            Address = p.Address,
            Telephone = p.Telephone,
            Website = p.Website,
            Email = p.Email,
            PlaceId = p.PlaceId
        }).ToList();

        if (query.AddAllItemInFirstRow)
        {
            response.Insert(0, new PublisherResponse { Id = 0, Name = "همه ناشران", ManagerName = "", Telephone = "", Address = "", Website = "", Email = "", PlaceId = 0 });
        }

        return Task.FromResult(Result.Success(response));
    }
}


