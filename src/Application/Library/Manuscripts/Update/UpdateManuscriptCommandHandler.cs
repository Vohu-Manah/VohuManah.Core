using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Manuscripts.Update;

internal sealed class UpdateManuscriptCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateManuscriptCommand>
{
    public async Task<Result> Handle(UpdateManuscriptCommand command, CancellationToken cancellationToken)
    {
        DbSet<Manuscript> manuscripts = unitOfWork.Set<Manuscript>();
        
        Manuscript? existing = await manuscripts
            .SingleOrDefaultAsync(m => m.Id == command.Id, cancellationToken);

        if (existing == null)
        {
            return Result.Failure(ManuscriptErrors.NotFound(command.Id));
        }

        string name = command.Name.Trim();
        if (await manuscripts.AnyAsync(m => m.Id != command.Id && m.Name.Trim() == name, cancellationToken))
        {
            return Result.Failure(ManuscriptErrors.NameNotUnique);
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

