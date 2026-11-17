using VohuManah.Application.Abstractions.Authentication;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Users;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Users.ChangePassword;

internal sealed class ChangePasswordCommandHandler(
    IApplicationDbContext dbContext,
    IPasswordHasher passwordHasher) : ICommandHandler<ChangePasswordCommand>
{
    public async Task<Result> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        var users = dbContext.LibraryUsers;

        var user = await users
            .FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);

        if (user == null)
        {
            return Result.Failure(UserErrors.NotFound(command.UserId));
        }

        if (!passwordHasher.Verify(command.CurrentPassword, user.Password))
        {
            return Result.Failure(Error.Validation("Users.InvalidCurrentPassword", "رمز عبور فعلی صحیح نیست"));
        }

        string newPasswordHash = passwordHasher.Hash(command.NewPassword);
        user.Password = newPasswordHash;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

