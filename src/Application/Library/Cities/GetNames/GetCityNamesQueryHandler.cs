using Application.Library._Shared;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Cities.GetNames;

internal sealed class GetCityNamesQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetCityNamesQuery, List<SelectItemResponse>>
{
    public Task<Result<List<SelectItemResponse>>> Handle(GetCityNamesQuery query, CancellationToken cancellationToken)
    {
        DbSet<City> cities = unitOfWork.Set<City>();

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


