using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;
using VohuManah.Domain.Cities;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Cities.GetNames;

internal sealed class GetCityNamesQueryHandler(
    IApplicationDbContext dbContext) : IQueryHandler<GetCityNamesQuery, List<SelectItemResponse>>
{
    public Task<Result<List<SelectItemResponse>>> Handle(GetCityNamesQuery query, CancellationToken cancellationToken)
    {
        DbSet<City> cities = dbContext.Cities;

        List<SelectItemResponse> items = cities
            .OrderBy(c => c.Name)
            .Select(c => new SelectItemResponse { Id = c.Id, Title = c.Name })
            .ToList();

        if (query.AddAllItemInFirstRow)
        {
            items.Insert(0, new SelectItemResponse { Id = 0, Title = "همه شهرها" });
        }

        return Task.FromResult(Result.Success(items));
    }
}


