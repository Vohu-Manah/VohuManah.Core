using VohuManah.Application.Abstractions.Authentication;
using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Library;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Users.Create;

internal sealed class CreateLibraryUserCommandHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager,
    IPasswordHasher passwordHasher) : ICommandHandler<CreateLibraryUserCommand, string>
{
    private const string UserPatternKey = "sdp.user.";

    public async Task<Result<string>> Handle(CreateLibraryUserCommand command, CancellationToken cancellationToken)
    {
        DbSet<User> users = unitOfWork.GetDbSet<User>();

        if (await users.AnyAsync(u => u.UserName == command.UserName, cancellationToken))
        {
            return Result.Failure<string>(UserErrors.UserNameNotUnique);
        }

        string passwordHash = passwordHasher.Hash(command.Password);

        var user = new User
        {
            UserName = command.UserName,
            Password = passwordHash,
            Name = command.Name,
            LastName = command.LastName
        };

        users.Add(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        cacheManager.RemoveByPattern(UserPatternKey);

        return user.UserName;
    }
}


