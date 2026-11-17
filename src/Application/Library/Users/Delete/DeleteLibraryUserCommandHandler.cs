using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Library;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Users.Delete;

internal sealed class DeleteLibraryUserCommandHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : ICommandHandler<DeleteLibraryUserCommand>
{
    private const string UserPatternKey = "sdp.user.";

    public async Task<Result> Handle(DeleteLibraryUserCommand command, CancellationToken cancellationToken)
    {
        DbSet<VohuManah.Domain.Library.User> users = unitOfWork.GetDbSet<VohuManah.Domain.Library.User>();

        List<VohuManah.Domain.Library.User> allUsers = await users.ToListAsync(cancellationToken);
        if (allUsers.Count == 1)
        {
            return Result.Failure(UserErrors.CannotDeleteLastUser);
        }

        
        VohuManah.Domain.Library.User? existing = await users
            .SingleOrDefaultAsync(u => u.Id == command.Id, cancellationToken);

        if (existing == null)
        {
            return Result.Failure(UserErrors.NotFound(command.Id.ToString()));
        }

        users.Remove(existing);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        cacheManager.RemoveByPattern(UserPatternKey);

        return Result.Success();
    }
}


