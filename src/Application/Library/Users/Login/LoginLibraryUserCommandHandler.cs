using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.HumanVerification;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SharedKernel;

namespace Application.Library.Users.Login;

internal sealed class LoginLibraryUserCommandHandler(
    IUnitOfWork unitOfWork,
    ILibraryTokenProvider tokenProvider,
    IPasswordHasher passwordHasher,
    IConfiguration configuration,
    IHumanVerificationService humanVerification) : ICommandHandler<LoginLibraryUserCommand, LoginLibraryUserResponse>
{
    public async Task<Result<LoginLibraryUserResponse>> Handle(LoginLibraryUserCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(command.HumanVerifyToken) ||
            !await humanVerification.ConfirmTokenValidAsync(command.HumanVerifyToken, cancellationToken))
        {
            return Result.Failure<LoginLibraryUserResponse>(HumanVerifyErrors.InvalidOrExpired);
        }

        DbSet<Domain.Library.User> users = unitOfWork.GetDbSet<Domain.Library.User>();

        string loginInput = command.UserName.Trim();
        Domain.Library.User? user = await users
            .FirstOrDefaultAsync(u => u.UserName == loginInput || (u.Email != null && u.Email == loginInput), cancellationToken);

        if (user == null || !passwordHasher.Verify(command.Password.Trim(), user.Password))
        {
            return Result.Failure<LoginLibraryUserResponse>(UserErrors.InvalidCredentials);
        }

        string token = await tokenProvider.CreateAsync(user, cancellationToken);

        int refreshDays = configuration.GetValue<int>("Jwt:RefreshTokenDays", 7);

        RefreshToken refresh = new RefreshToken
        {
            UserName = user.UserName,
            Token = Guid.NewGuid().ToString("N"),
            ExpiresAtUtc = DateTime.UtcNow.AddDays(refreshDays)
        };
        unitOfWork.GetDbSet<RefreshToken>().Add(refresh);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        LoginLibraryUserResponse response = new LoginLibraryUserResponse
        {
            UserName = user.UserName,
            FullName = $"{user.Name} {user.LastName}",
            AccessToken = token,
            RefreshToken = refresh.Token
        };

        return Result.Success(response);
    }
}


