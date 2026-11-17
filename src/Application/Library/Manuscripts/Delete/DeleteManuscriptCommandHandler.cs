using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Library;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Manuscripts.Delete;

internal sealed class DeleteManuscriptCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteManuscriptCommand>
{
    public async Task<Result> Handle(DeleteManuscriptCommand command, CancellationToken cancellationToken)
    {
        DbSet<Manuscript> manuscripts = unitOfWork.GetDbSet<Manuscript>();

        Manuscript? manuscript = await manuscripts
            .SingleOrDefaultAsync(m => m.Id == command.Id, cancellationToken);

        if (manuscript == null)
        {
            return Result.Failure(ManuscriptErrors.NotFound(command.Id));
        }

        manuscripts.Remove(manuscript);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

