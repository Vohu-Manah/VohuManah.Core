using Application.Abstractions.Authentication;
using Application.Abstractions.Caching;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Users.Update;

internal sealed class UpdateLibraryUserCommandHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager,
    IPasswordHasher passwordHasher) : ICommandHandler<UpdateLibraryUserCommand>
{
    private const string UserPatternKey = "sdp.user.";

    public async Task<Result> Handle(UpdateLibraryUserCommand command, CancellationToken cancellationToken)
    {
        DbSet<Domain.Library.User> users = unitOfWork.GetDbSet<Domain.Library.User>();

        Domain.Library.User? existing = await users
            .SingleOrDefaultAsync(u => u.Id == command.Id, cancellationToken);

        if (existing == null)
        {
            return Result.Failure(UserErrors.NotFound(command.Id.ToString()));
        }

        string passwordHash = passwordHasher.Hash(command.Password);
        existing.Password = passwordHash;
        existing.Name = command.Name;
        existing.LastName = command.LastName;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        cacheManager.RemoveByPattern(UserPatternKey);

        return Result.Success();
    }
}


