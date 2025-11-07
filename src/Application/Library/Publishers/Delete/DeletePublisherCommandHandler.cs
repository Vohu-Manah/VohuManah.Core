using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Application.Abstractions.Caching;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Publishers.Delete;

internal sealed class DeletePublisherCommandHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : ICommandHandler<DeletePublisherCommand>
{
    private const string PublisherPatternKey = "sdp.publisher.";

    public async Task<Result> Handle(DeletePublisherCommand command, CancellationToken cancellationToken)
    {
        DbSet<Publisher> publishers = unitOfWork.Set<Publisher>();
        
        Publisher? existing = await publishers.SingleOrDefaultAsync(p => p.Id == command.Id, cancellationToken);
        if (existing == null)
        {
            return Result.Failure(Error.NotFound("Publishers.NotFound", $"Publisher with Id {command.Id} not found"));
        }

        publishers.Remove(existing);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        cacheManager.RemoveByPattern(PublisherPatternKey);

        return Result.Success();
    }
}


