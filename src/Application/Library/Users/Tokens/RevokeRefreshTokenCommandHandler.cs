using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Library;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Users.Tokens;

internal sealed class RevokeRefreshTokenCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<RevokeRefreshTokenCommand>
{
    public async Task<Result> Handle(RevokeRefreshTokenCommand command, CancellationToken cancellationToken)
    {
        RefreshToken? refresh = await unitOfWork.GetDbSet<RefreshToken>()
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


