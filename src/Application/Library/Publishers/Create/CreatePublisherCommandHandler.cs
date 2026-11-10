using Application.Abstractions.Caching;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Publishers.Create;

internal sealed class CreatePublisherCommandHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : ICommandHandler<CreatePublisherCommand, int>
{
    private const string PublisherPatternKey = "sdp.publisher.";

    public async Task<Result<int>> Handle(CreatePublisherCommand command, CancellationToken cancellationToken)
    {
        DbSet<Publisher> publishers = unitOfWork.GetDbSet<Publisher>();

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
        await unitOfWork.SaveChangesAsync(cancellationToken);

        cacheManager.RemoveByPattern(PublisherPatternKey);

        return publisher.Id;
    }
}


