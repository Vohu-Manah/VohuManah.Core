using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Application.Abstractions.Caching;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Users.Delete;

internal sealed class DeleteLibraryUserCommandHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : ICommandHandler<DeleteLibraryUserCommand>
{
    private const string UserPatternKey = "sdp.user.";

    public async Task<Result> Handle(DeleteLibraryUserCommand command, CancellationToken cancellationToken)
    {
        DbSet<Domain.Library.User> users = unitOfWork.Set<Domain.Library.User>();
        
        List<Domain.Library.User> allUsers = await users.ToListAsync(cancellationToken);
        if (allUsers.Count == 1)
        {
            return Result.Failure(UserErrors.CannotDeleteLastUser);
        }

        Domain.Library.User? existing = await users
            .SingleOrDefaultAsync(u => u.UserName == command.UserName, cancellationToken);

        if (existing == null)
        {
            return Result.Failure(UserErrors.NotFound(command.UserName));
        }

        users.Remove(existing);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        cacheManager.RemoveByPattern(UserPatternKey);

        return Result.Success();
    }
}


