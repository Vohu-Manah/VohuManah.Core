using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Cities;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Cities.Update;

internal sealed class UpdateCityCommandHandler(
    IApplicationDbContext dbContext,
    ICacheManager cacheManager) : ICommandHandler<UpdateCityCommand>
{
    private const string CityPatternKey = "sdp.city.";

    public async Task<Result> Handle(UpdateCityCommand command, CancellationToken cancellationToken)
    {
        DbSet<City> cities = dbContext.Cities;

        City? existing = await cities.SingleOrDefaultAsync(c => c.Id == command.Id, cancellationToken);
        if (existing == null)
        {
            return Result.Failure(Error.NotFound("Cities.NotFound", $"City with Id {command.Id} not found"));
        }

        if (await cities.AnyAsync(c => c.Id != command.Id && c.Name == command.Name, cancellationToken))
        {
            return Result.Failure(CityErrors.NameNotUnique);
        }

        existing.Name = command.Name;
        await dbContext.SaveChangesAsync(cancellationToken);

        cacheManager.RemoveByPattern(CityPatternKey);

        return Result.Success();
    }
}


