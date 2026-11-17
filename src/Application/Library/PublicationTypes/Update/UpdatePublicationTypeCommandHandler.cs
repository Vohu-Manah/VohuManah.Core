using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.PublicationTypes;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.PublicationTypes.Update;

internal sealed class UpdatePublicationTypeCommandHandler(
    IApplicationDbContext dbContext,
    ICacheManager cacheManager) : ICommandHandler<UpdatePublicationTypeCommand>
{
    private const string PublicationTypePatternKey = "sdp.publicationType.";

    public async Task<Result> Handle(UpdatePublicationTypeCommand command, CancellationToken cancellationToken)
    {
        DbSet<PublicationType> publicationTypes = dbContext.PublicationTypes;

        PublicationType? existing = await publicationTypes
            .SingleOrDefaultAsync(pt => pt.Id == command.Id, cancellationToken);

        if (existing == null)
        {
            return Result.Failure(PublicationTypeErrors.NotFound(command.Id));
        }

        if (await publicationTypes.AnyAsync(pt => pt.Id != command.Id && pt.Title == command.Title, cancellationToken))
        {
            return Result.Failure(PublicationTypeErrors.TitleNotUnique);
        }

        existing.Title = command.Title;

        await dbContext.SaveChangesAsync(cancellationToken);

        cacheManager.RemoveByPattern(PublicationTypePatternKey);

        return Result.Success();
    }
}

