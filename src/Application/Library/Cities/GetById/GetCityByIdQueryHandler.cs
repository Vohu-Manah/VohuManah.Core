using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Cities;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Cities.GetById;

internal sealed class GetCityByIdQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetCityByIdQuery, CityResponse>
{
    public async Task<Result<CityResponse>> Handle(GetCityByIdQuery query, CancellationToken cancellationToken)
    {
        DbSet<City> cities = unitOfWork.GetDbSet<City>();

        City? city = await cities
            .SingleOrDefaultAsync(c => c.Id == query.Id, cancellationToken);

        if (city == null)
        {
            return Result.Failure<CityResponse>(CityErrors.NotFound(query.Id));
        }

        CityResponse response = new CityResponse
        {
            Id = city.Id,
            Name = city.Name
        };

        return Result.Success(response);
    }
}

