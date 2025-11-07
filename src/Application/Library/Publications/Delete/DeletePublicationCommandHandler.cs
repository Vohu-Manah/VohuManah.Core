using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Publications.Delete;

internal sealed class DeletePublicationCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<DeletePublicationCommand>
{
    public async Task<Result> Handle(DeletePublicationCommand command, CancellationToken cancellationToken)
    {
        DbSet<Publication> publications = unitOfWork.Set<Publication>();
        
        Publication? publication = await publications
            .SingleOrDefaultAsync(p => p.Id == command.Id, cancellationToken);

        if (publication == null)
        {
            return Result.Failure(PublicationErrors.NotFound(command.Id));
        }

        publications.Remove(publication);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

