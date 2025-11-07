using Application.Abstractions.Caching;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.PublicationTypes.Update;

internal sealed class UpdatePublicationTypeCommandHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : ICommandHandler<UpdatePublicationTypeCommand>
{
    private const string PublicationTypePatternKey = "sdp.publicationType.";

    public async Task<Result> Handle(UpdatePublicationTypeCommand command, CancellationToken cancellationToken)
    {
        DbSet<PublicationType> publicationTypes = unitOfWork.Set<PublicationType>();
        
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
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        cacheManager.RemoveByPattern(PublicationTypePatternKey);

        return Result.Success();
    }
}

