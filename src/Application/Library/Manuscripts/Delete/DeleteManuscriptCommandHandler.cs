using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Manuscripts;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Manuscripts.Delete;

internal sealed class DeleteManuscriptCommandHandler(
    IApplicationDbContext dbContext) : ICommandHandler<DeleteManuscriptCommand>
{
    public async Task<Result> Handle(DeleteManuscriptCommand command, CancellationToken cancellationToken)
    {
        DbSet<Manuscript> manuscripts = dbContext.Manuscripts;

        Manuscript? manuscript = await manuscripts
            .SingleOrDefaultAsync(m => m.Id == command.Id, cancellationToken);

        if (manuscript == null)
        {
            return Result.Failure(ManuscriptErrors.NotFound(command.Id));
        }

        manuscripts.Remove(manuscript);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

