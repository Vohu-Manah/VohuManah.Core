using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Users.ChangePassword;

internal sealed class ChangePasswordCommandHandler(
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher) : ICommandHandler<ChangePasswordCommand>
{
    public async Task<Result> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        var users = unitOfWork.GetDbSet<User>();

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

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

