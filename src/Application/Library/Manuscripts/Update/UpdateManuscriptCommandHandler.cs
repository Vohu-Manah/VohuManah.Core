using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Library;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Manuscripts.Update;

internal sealed class UpdateManuscriptCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateManuscriptCommand>
{
    public async Task<Result> Handle(UpdateManuscriptCommand command, CancellationToken cancellationToken)
    {
        DbSet<Manuscript> manuscripts = unitOfWork.GetDbSet<Manuscript>();

        Manuscript? existing = await manuscripts
            .SingleOrDefaultAsync(m => m.Id == command.Id, cancellationToken);

        if (existing == null)
        {
            return Result.Failure(ManuscriptErrors.NotFound(command.Id));
        }


        existing.Name = command.Name;
        existing.Author = command.Author;
        existing.GapId = command.GapId;
        existing.LanguageId = command.LanguageId;
        existing.PageCount = command.PageCount;
        existing.Size = command.Size;
        existing.SubjectId = command.SubjectId;
        existing.VersionNo = command.VersionNo;
        existing.WritingPlaceId = command.WritingPlaceId;
        existing.Year = command.Year;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

