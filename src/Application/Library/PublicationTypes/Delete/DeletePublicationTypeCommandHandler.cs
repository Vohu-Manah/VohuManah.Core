using Application.Abstractions.Caching;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.PublicationTypes.Delete;

internal sealed class DeletePublicationTypeCommandHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : ICommandHandler<DeletePublicationTypeCommand>
{
    private const string PublicationTypePatternKey = "sdp.publicationType.";

    public async Task<Result> Handle(DeletePublicationTypeCommand command, CancellationToken cancellationToken)
    {
        DbSet<PublicationType> publicationTypes = unitOfWork.Set<PublicationType>();
        
        PublicationType? publicationType = await publicationTypes
            .SingleOrDefaultAsync(pt => pt.Id == command.Id, cancellationToken);

        if (publicationType == null)
        {
            return Result.Failure(PublicationTypeErrors.NotFound(command.Id));
        }

        publicationTypes.Remove(publicationType);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        cacheManager.RemoveByPattern(PublicationTypePatternKey);

        return Result.Success();
    }
}

