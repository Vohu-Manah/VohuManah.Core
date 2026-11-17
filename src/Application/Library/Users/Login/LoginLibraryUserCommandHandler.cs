using VohuManah.Application.Abstractions.Authentication;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.HumanVerification;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Library;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Users.Login;

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

        DbSet<VohuManah.Domain.Library.User> users = unitOfWork.GetDbSet<VohuManah.Domain.Library.User>();

        string loginInput = command.UserName.Trim();
        VohuManah.Domain.Library.User? user = await users
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


