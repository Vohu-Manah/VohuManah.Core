using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Publications.Update;

internal sealed class UpdatePublicationCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<UpdatePublicationCommand>
{
    public async Task<Result> Handle(UpdatePublicationCommand command, CancellationToken cancellationToken)
    {
        DbSet<Publication> publications = unitOfWork.Set<Publication>();
        
        Publication? existing = await publications
            .SingleOrDefaultAsync(p => p.Id == command.Id, cancellationToken);

        if (existing == null)
        {
            return Result.Failure(PublicationErrors.NotFound(command.Id));
        }

        if (await publications.AnyAsync(p => p.Id != command.Id && p.No == command.No, cancellationToken))
        {
            return Result.Failure(PublicationErrors.NoNotUnique);
        }

        existing.Name = command.Name;
        existing.Concessionaire = command.Concessionaire;
        existing.Editor = command.Editor;
        existing.LanguageId = command.LanguageId;
        existing.JournalNo = command.JournalNo;
        existing.No = command.No;
        existing.SubjectId = command.SubjectId;
        existing.PublishDate = command.PublishDate;
        existing.PublishPlaceId = command.PublishPlaceId;
        existing.Year = command.Year;
        existing.ResponsibleDirector = command.ResponsibleDirector;
        existing.TypeId = command.TypeId;
        existing.Period = command.Period;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

