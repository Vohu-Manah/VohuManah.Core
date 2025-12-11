using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Application.Abstractions.Authentication;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using Microsoft.Extensions.Configuration;

namespace Application.Library.Users.Tokens;

internal sealed class RefreshAccessTokenCommandHandler(
    IUnitOfWork unitOfWork,
    ILibraryTokenProvider tokenProvider,
    IConfiguration configuration) : ICommandHandler<RefreshAccessTokenCommand, RefreshAccessTokenResponse>
{
    public async Task<Result<RefreshAccessTokenResponse>> Handle(RefreshAccessTokenCommand command, CancellationToken cancellationToken)
    {
        RefreshToken? refresh = await unitOfWork.Set<RefreshToken>()
            .SingleOrDefaultAsync(r => r.Token == command.RefreshToken, cancellationToken);

        if (refresh == null || !refresh.IsActive)
        {
            return Result.Failure<RefreshAccessTokenResponse>(Error.Conflict("Auth.InvalidRefresh", "Invalid or expired refresh token"));
        }

        User? user = await unitOfWork.Set<User>()
            .SingleOrDefaultAsync(u => u.UserName == refresh.UserName, cancellationToken);

        if (user == null)
        {
            return Result.Failure<RefreshAccessTokenResponse>(Error.NotFound("Users.NotFound", "User not found"));
        }

        refresh.RevokedAtUtc = DateTime.UtcNow;

        string accessToken = await tokenProvider.CreateAsync(user, cancellationToken);

        int refreshDays = configuration.GetValue<int>("Jwt:RefreshTokenDays", 7);

        RefreshToken newRefresh = new RefreshToken
        {
            UserName = user.UserName,
            Token = Guid.NewGuid().ToString("N"),
            ExpiresAtUtc = DateTime.UtcNow.AddDays(refreshDays)
        };

        unitOfWork.Set<RefreshToken>().Add(newRefresh);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new RefreshAccessTokenResponse(accessToken, newRefresh.Token);
        return Result.Success(response);
    }
}


