using VohuManah.Application.Abstractions.Authentication;
using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Users;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Users.Update;

internal sealed class UpdateLibraryUserCommandHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager,
    IPasswordHasher passwordHasher) : ICommandHandler<UpdateLibraryUserCommand>
{
    private const string UserPatternKey = "sdp.user.";

    public async Task<Result> Handle(UpdateLibraryUserCommand command, CancellationToken cancellationToken)
    {
        DbSet<VohuManah.Domain.Users.User> users = unitOfWork.GetDbSet<VohuManah.Domain.Users.User>();

        VohuManah.Domain.Users.User? existing = await users
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


