using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Publishers;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Publishers.Create;

internal sealed class CreatePublisherCommandHandler(
    IApplicationDbContext dbContext,
    ICacheManager cacheManager) : ICommandHandler<CreatePublisherCommand, int>
{
    private const string PublisherPatternKey = "sdp.publisher.";

    public async Task<Result<int>> Handle(CreatePublisherCommand command, CancellationToken cancellationToken)
    {
        DbSet<Publisher> publishers = dbContext.Publishers;

        if (await publishers.AnyAsync(p => p.Name == command.Name, cancellationToken))
        {
            return Result.Failure<int>(PublisherErrors.NameNotUnique);
        }

        var publisher = new Publisher
        {
            Name = command.Name,
            ManagerName = command.ManagerName,
            PlaceId = command.PlaceId,
            Address = command.Address,
            Telephone = command.Telephone,
            Website = command.Website,
            Email = command.Email
        };

        publishers.Add(publisher);
        await dbContext.SaveChangesAsync(cancellationToken);

        cacheManager.RemoveByPattern(PublisherPatternKey);

        return publisher.Id;
    }
}


