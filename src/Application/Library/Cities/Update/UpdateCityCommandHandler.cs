using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Application.Abstractions.Caching;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Cities.Update;

internal sealed class UpdateCityCommandHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : ICommandHandler<UpdateCityCommand>
{
    private const string CityPatternKey = "sdp.city.";

    public async Task<Result> Handle(UpdateCityCommand command, CancellationToken cancellationToken)
    {
        DbSet<City> cities = unitOfWork.Set<City>();
        
        City? existing = await cities.SingleOrDefaultAsync(c => c.Id == command.Id, cancellationToken);
        if (existing == null)
        {
            return Result.Failure(Error.NotFound("Cities.NotFound", $"City with Id {command.Id} not found"));
        }

        if (await cities.AnyAsync(c => c.Id != command.Id && c.Name == command.Name, cancellationToken))
        {
            return Result.Failure(Error.Conflict("Cities.NameNotUnique", "A city with this name already exists"));
        }

        existing.Name = command.Name;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        cacheManager.RemoveByPattern(CityPatternKey);

        return Result.Success();
    }
}


