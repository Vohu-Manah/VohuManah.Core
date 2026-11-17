using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Publishers;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Publishers.Delete;

internal sealed class DeletePublisherCommandHandler(
    IApplicationDbContext dbContext,
    ICacheManager cacheManager) : ICommandHandler<DeletePublisherCommand>
{
    private const string PublisherPatternKey = "sdp.publisher.";

    public async Task<Result> Handle(DeletePublisherCommand command, CancellationToken cancellationToken)
    {
        DbSet<Publisher> publishers = dbContext.Publishers;

        Publisher? existing = await publishers.SingleOrDefaultAsync(p => p.Id == command.Id, cancellationToken);
        if (existing == null)
        {
            return Result.Failure(Error.NotFound("Publishers.NotFound", $"Publisher with Id {command.Id} not found"));
        }

        publishers.Remove(existing);
        await dbContext.SaveChangesAsync(cancellationToken);

        cacheManager.RemoveByPattern(PublisherPatternKey);

        return Result.Success();
    }
}


