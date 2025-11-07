using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Application.Abstractions.Caching;
using Application.Abstractions.Authentication;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Users.Create;

internal sealed class CreateLibraryUserCommandHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager,
    IPasswordHasher passwordHasher) : ICommandHandler<CreateLibraryUserCommand, string>
{
    private const string UserPatternKey = "sdp.user.";

    public async Task<Result<string>> Handle(CreateLibraryUserCommand command, CancellationToken cancellationToken)
    {
        DbSet<Domain.Library.User> users = unitOfWork.Set<Domain.Library.User>();
        
        if (await users.AnyAsync(u => u.UserName == command.UserName, cancellationToken))
        {
            return Result.Failure<string>(UserErrors.UserNameNotUnique);
        }

        string passwordHash = passwordHasher.Hash(command.Password);

        Domain.Library.User user = new Domain.Library.User {
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


