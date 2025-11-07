using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Application.Abstractions.Caching;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Publishers.Update;

internal sealed class UpdatePublisherCommandHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : ICommandHandler<UpdatePublisherCommand>
{
    private const string PublisherPatternKey = "sdp.publisher.";

    public async Task<Result> Handle(UpdatePublisherCommand command, CancellationToken cancellationToken)
    {
        DbSet<Publisher> publishers = unitOfWork.Set<Publisher>();
        
        Publisher? existing = await publishers.SingleOrDefaultAsync(p => p.Id == command.Id, cancellationToken);
        if (existing == null)
        {
            return Result.Failure(Error.NotFound("Publishers.NotFound", $"Publisher with Id {command.Id} not found"));
        }

        if (await publishers.AnyAsync(p => p.Id != command.Id && p.Name == command.Name, cancellationToken))
        {
            return Result.Failure(Error.Conflict("Publishers.NameNotUnique", "A publisher with this name already exists"));
        }

        existing.Name = command.Name;
        existing.Address = command.Address;
        existing.Email = command.Email;
        existing.ManagerName = command.ManagerName;
        existing.PlaceId = command.PlaceId;
        existing.Telephone = command.Telephone;
        existing.Website = command.Website;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        cacheManager.RemoveByPattern(PublisherPatternKey);

        return Result.Success();
    }
}


