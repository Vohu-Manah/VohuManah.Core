using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Cities;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Cities.Delete;

internal sealed class DeleteCityCommandHandler(
    IApplicationDbContext dbContext,
    ICacheManager cacheManager) : ICommandHandler<DeleteCityCommand>
{
    private const string CityPatternKey = "sdp.city.";

    public async Task<Result> Handle(DeleteCityCommand command, CancellationToken cancellationToken)
    {
        DbSet<City> cities = dbContext.Cities;

        City? existing = await cities.SingleOrDefaultAsync(c => c.Id == command.Id, cancellationToken);
        if (existing == null)
        {
            return Result.Failure(Error.NotFound("Cities.NotFound", $"شهر با شناسه {command.Id} یافت نشد"));
        }

        cities.Remove(existing);
        await dbContext.SaveChangesAsync(cancellationToken);

        cacheManager.RemoveByPattern(CityPatternKey);

        return Result.Success();
    }
}


