using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Cities;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Cities.Create;

internal sealed class CreateCityCommandHandler(
    IApplicationDbContext dbContext,
    ICacheManager cacheManager) : ICommandHandler<CreateCityCommand, int>
{
    private const string CityPatternKey = "sdp.city.";

    public async Task<Result<int>> Handle(CreateCityCommand command, CancellationToken cancellationToken)
    {
        DbSet<City> cities = dbContext.Cities;

        if (await cities.AnyAsync(c => c.Name == command.Name, cancellationToken))
        {
            return Result.Failure<int>(CityErrors.NameNotUnique);
        }

        var city = new City { Name = command.Name };
        cities.Add(city);
        await dbContext.SaveChangesAsync(cancellationToken);

        cacheManager.RemoveByPattern(CityPatternKey);

        return city.Id;
    }
}


