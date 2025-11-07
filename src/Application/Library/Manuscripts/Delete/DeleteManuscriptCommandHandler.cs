using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Manuscripts.Delete;

internal sealed class DeleteManuscriptCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteManuscriptCommand>
{
    public async Task<Result> Handle(DeleteManuscriptCommand command, CancellationToken cancellationToken)
    {
        DbSet<Manuscript> manuscripts = unitOfWork.Set<Manuscript>();
        
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

