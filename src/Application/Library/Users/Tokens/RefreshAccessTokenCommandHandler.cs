using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Application.Abstractions.Authentication;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Users.Tokens;

internal sealed class RefreshAccessTokenCommandHandler(
    IUnitOfWork unitOfWork,
    ILibraryTokenProvider tokenProvider) : ICommandHandler<RefreshAccessTokenCommand, string>
{
    public async Task<Result<string>> Handle(RefreshAccessTokenCommand command, CancellationToken cancellationToken)
    {
        RefreshToken? refresh = await unitOfWork.Set<RefreshToken>()
            .SingleOrDefaultAsync(r => r.Token == command.RefreshToken, cancellationToken);

        if (refresh == null || !refresh.IsActive)
        {
            return Result.Failure<string>(Error.Conflict("Auth.InvalidRefresh", "Invalid or expired refresh token"));
        }

        User? user = await unitOfWork.Set<User>()
            .SingleOrDefaultAsync(u => u.UserName == refresh.UserName, cancellationToken);

        if (user == null)
        {
            return Result.Failure<string>(Error.NotFound("Users.NotFound", "User not found"));
        }

        string access = await tokenProvider.CreateAsync(user, cancellationToken);
        return access;
    }
}


