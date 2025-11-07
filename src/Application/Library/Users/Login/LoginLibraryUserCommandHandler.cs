using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Application.Abstractions.Authentication;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Users.Login;

internal sealed class LoginLibraryUserCommandHandler(
    IUnitOfWork unitOfWork,
    ILibraryTokenProvider tokenProvider,
    IPasswordHasher passwordHasher) : ICommandHandler<LoginLibraryUserCommand, LoginLibraryUserResponse>
{
    public async Task<Result<LoginLibraryUserResponse>> Handle(LoginLibraryUserCommand command, CancellationToken cancellationToken)
    {
        DbSet<Domain.Library.User> users = unitOfWork.Set<Domain.Library.User>();
        
        Domain.Library.User? user = await users
            .SingleOrDefaultAsync(u => u.UserName == command.UserName.Trim(), cancellationToken);

        if (user == null || !passwordHasher.Verify(command.Password.Trim(), user.Password))
        {
            return Result.Failure<LoginLibraryUserResponse>(UserErrors.InvalidCredentials);
        }

        string token = await tokenProvider.CreateAsync(user, cancellationToken);

        // create refresh token
        RefreshToken refresh = new RefreshToken {
            UserName = user.UserName,
            Token = Guid.NewGuid().ToString("N"),
            ExpiresAtUtc = DateTime.UtcNow.AddDays(7)
        };
        unitOfWork.Set<RefreshToken>().Add(refresh);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        LoginLibraryUserResponse response = new LoginLibraryUserResponse {
            UserName = user.UserName,
            FullName = $"{user.Name} {user.LastName}",
            AccessToken = token,
            RefreshToken = refresh.Token
        };

        return Result.Success(response);
    }
}


