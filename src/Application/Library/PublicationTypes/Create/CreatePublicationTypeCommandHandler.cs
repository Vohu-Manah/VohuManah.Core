using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.PublicationTypes;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.PublicationTypes.Create;

internal sealed class CreatePublicationTypeCommandHandler(
    IApplicationDbContext dbContext,
    ICacheManager cacheManager) : ICommandHandler<CreatePublicationTypeCommand, int>
{
    private const string PublicationTypePatternKey = "sdp.publicationType.";

    public async Task<Result<int>> Handle(CreatePublicationTypeCommand command, CancellationToken cancellationToken)
    {
        DbSet<PublicationType> publicationTypes = dbContext.PublicationTypes;

        if (await publicationTypes.AnyAsync(pt => pt.Title == command.Title, cancellationToken))
        {
            return Result.Failure<int>(PublicationTypeErrors.TitleNotUnique);
        }

        var publicationType = new PublicationType
        {
            Title = command.Title
        };

        publicationTypes.Add(publicationType);
        await dbContext.SaveChangesAsync(cancellationToken);

        cacheManager.RemoveByPattern(PublicationTypePatternKey);

        return publicationType.Id;
    }
}

