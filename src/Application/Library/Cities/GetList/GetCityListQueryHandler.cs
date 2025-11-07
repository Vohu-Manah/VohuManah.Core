using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Caching;
using Domain.Library;
using SharedKernel;

namespace Application.Library.Cities.GetList;

internal sealed class GetCityListQueryHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : IQueryHandler<GetCityListQuery, List<CityListResponse>>
{
    private const string CityAllKey = "sdp.city.all";

    public Task<Result<List<CityListResponse>>> Handle(GetCityListQuery query, CancellationToken cancellationToken)
    {
        var cities = unitOfWork.Set<City>();
        
        var allCities = cacheManager.Get(CityAllKey, () => cities.OrderBy(c => c.Name).ToList());
        
        var list = allCities.Select(c => new CityListResponse
        {
            Id = c.Id,
            Name = c.Name
        }).ToList();

        return Task.FromResult(Result.Success(list));
    }
}

