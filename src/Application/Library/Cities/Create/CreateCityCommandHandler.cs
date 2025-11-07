using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Application.Abstractions.Caching;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Cities.Create;

internal sealed class CreateCityCommandHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : ICommandHandler<CreateCityCommand, int>
{
    private const string CityPatternKey = "sdp.city.";

    public async Task<Result<int>> Handle(CreateCityCommand command, CancellationToken cancellationToken)
    {
        DbSet<City> cities = unitOfWork.Set<City>();
        
        if (await cities.AnyAsync(c => c.Name == command.Name, cancellationToken))
        {
            return Result.Failure<int>(Error.Conflict("Cities.NameNotUnique", "A city with this name already exists"));
        }

        City city = new City { Name = command.Name };
        cities.Add(city);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        cacheManager.RemoveByPattern(CityPatternKey);

        return city.Id;
    }
}


