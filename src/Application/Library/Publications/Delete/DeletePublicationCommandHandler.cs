using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Publications;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Publications.Delete;

internal sealed class DeletePublicationCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<DeletePublicationCommand>
{
    public async Task<Result> Handle(DeletePublicationCommand command, CancellationToken cancellationToken)
    {
        DbSet<Publication> publications = unitOfWork.GetDbSet<Publication>();

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

