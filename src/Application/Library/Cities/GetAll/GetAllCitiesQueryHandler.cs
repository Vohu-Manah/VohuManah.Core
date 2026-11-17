using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Cities;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Cities.GetAll;

internal sealed class GetAllCitiesQueryHandler(
    IApplicationDbContext dbContext,
    ICacheManager cacheManager) : IQueryHandler<GetAllCitiesQuery, List<CityResponse>>
{
    private const string CityAllKey = "sdp.city.all";

    public Task<Result<List<CityResponse>>> Handle(GetAllCitiesQuery query, CancellationToken cancellationToken)
    {
        DbSet<City> cities = dbContext.Cities;

        List<City> allCities = cacheManager.Retrieve(CityAllKey, () =>
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


