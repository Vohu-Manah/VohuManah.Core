using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Publishers;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Publishers.Update;

internal sealed class UpdatePublisherCommandHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : ICommandHandler<UpdatePublisherCommand>
{
    private const string PublisherPatternKey = "sdp.publisher.";

    public async Task<Result> Handle(UpdatePublisherCommand command, CancellationToken cancellationToken)
    {
        DbSet<Publisher> publishers = unitOfWork.GetDbSet<Publisher>();

        Publisher? existing = await publishers.SingleOrDefaultAsync(p => p.Id == command.Id, cancellationToken);
        if (existing == null)
        {
            return Result.Failure(Error.NotFound("Publishers.NotFound", $"Publisher with Id {command.Id} not found"));
        }

        if (await publishers.AnyAsync(p => p.Id != command.Id && p.Name == command.Name, cancellationToken))
        {
            return Result.Failure(PublisherErrors.NameNotUnique);
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


