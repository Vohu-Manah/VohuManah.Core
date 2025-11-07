using Application.Abstractions.Caching;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.PublicationTypes.Create;

internal sealed class CreatePublicationTypeCommandHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : ICommandHandler<CreatePublicationTypeCommand, int>
{
    private const string PublicationTypePatternKey = "sdp.publicationType.";

    public async Task<Result<int>> Handle(CreatePublicationTypeCommand command, CancellationToken cancellationToken)
    {
        DbSet<PublicationType> publicationTypes = unitOfWork.Set<PublicationType>();
        
        if (await publicationTypes.AnyAsync(pt => pt.Title == command.Title, cancellationToken))
        {
            return Result.Failure<int>(PublicationTypeErrors.TitleNotUnique);
        }

        PublicationType publicationType = new PublicationType {
            Title = command.Title
        };

        publicationTypes.Add(publicationType);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        cacheManager.RemoveByPattern(PublicationTypePatternKey);

        return publicationType.Id;
    }
}

