using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Library;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.PublicationTypes.Delete;

internal sealed class DeletePublicationTypeCommandHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : ICommandHandler<DeletePublicationTypeCommand>
{
    private const string PublicationTypePatternKey = "sdp.publicationType.";

    public async Task<Result> Handle(DeletePublicationTypeCommand command, CancellationToken cancellationToken)
    {
        DbSet<PublicationType> publicationTypes = unitOfWork.GetDbSet<PublicationType>();

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

