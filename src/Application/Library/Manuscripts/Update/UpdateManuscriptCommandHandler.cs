using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Manuscripts;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Manuscripts.Update;

internal sealed class UpdateManuscriptCommandHandler(
    IApplicationDbContext dbContext) : ICommandHandler<UpdateManuscriptCommand>
{
    public async Task<Result> Handle(UpdateManuscriptCommand command, CancellationToken cancellationToken)
    {
        DbSet<Manuscript> manuscripts = dbContext.Manuscripts;

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

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

