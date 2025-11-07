using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Publications.Create;

internal sealed class CreatePublicationCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<CreatePublicationCommand, int>
{
    public async Task<Result<int>> Handle(CreatePublicationCommand command, CancellationToken cancellationToken)
    {
        DbSet<Publication> publications = unitOfWork.Set<Publication>();
        
        if (await publications.AnyAsync(p => p.No == command.No, cancellationToken))
        {
            return Result.Failure<int>(PublicationErrors.NoNotUnique);
        }

        Publication publication = new Publication {
            Name = command.Name,
            TypeId = command.TypeId,
            Concessionaire = command.Concessionaire,
            ResponsibleDirector = command.ResponsibleDirector,
            Editor = command.Editor,
            Year = command.Year,
            JournalNo = command.JournalNo,
            PublishDate = command.PublishDate,
            PublishPlaceId = command.PublishPlaceId,
            No = command.No,
            Period = command.Period,
            LanguageId = command.LanguageId,
            SubjectId = command.SubjectId
        };

        publications.Add(publication);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return publication.Id;
    }
}

