using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Application.Abstractions.Caching;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Cities.GetAll;

internal sealed class GetAllCitiesQueryHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : IQueryHandler<GetAllCitiesQuery, List<CityResponse>>
{
    private const string CityAllKey = "sdp.city.all";

    public Task<Result<List<CityResponse>>> Handle(GetAllCitiesQuery query, CancellationToken cancellationToken)
    {
        DbSet<City> cities = unitOfWork.Set<City>();
        
        List<City> allCities = cacheManager.Get(CityAllKey, () => 
            cities.OrderBy(c => c.Name).ToList());
        
        List<CityResponse> response = allCities.Select(c => new CityResponse
        {
            Id = c.Id,
            Name = c.Name
        }).ToList();

        if (query.AddAllItemInFirstRow)
        {
            response.Insert(0, new CityResponse { Id = 0, Name = "همه شهرها" });
        }

        return Task.FromResult(Result.Success(response));
    }
}


