using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Application.Abstractions.Caching;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Cities.Delete;

internal sealed class DeleteCityCommandHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : ICommandHandler<DeleteCityCommand>
{
    private const string CityPatternKey = "sdp.city.";

    public async Task<Result> Handle(DeleteCityCommand command, CancellationToken cancellationToken)
    {
        DbSet<City> cities = unitOfWork.Set<City>();
        
        City? existing = await cities.SingleOrDefaultAsync(c => c.Id == command.Id, cancellationToken);
        if (existing == null)
        {
            return Result.Failure(Error.NotFound("Cities.NotFound", $"City with Id {command.Id} not found"));
        }

        cities.Remove(existing);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        cacheManager.RemoveByPattern(CityPatternKey);

        return Result.Success();
    }
}


