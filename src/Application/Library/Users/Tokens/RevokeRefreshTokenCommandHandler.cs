using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Users.Tokens;

internal sealed class RevokeRefreshTokenCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<RevokeRefreshTokenCommand>
{
    public async Task<Result> Handle(RevokeRefreshTokenCommand command, CancellationToken cancellationToken)
    {
        RefreshToken? refresh = await unitOfWork.Set<RefreshToken>()
            .SingleOrDefaultAsync(r => r.Token == command.RefreshToken, cancellationToken);

        if (refresh == null)
        {
            return Result.Failure(Error.NotFound("Auth.RefreshNotFound", "Refresh token not found"));
        }

        refresh.RevokedAtUtc = DateTime.UtcNow;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}


