using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Library;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Settings.RemoveRoleFromUser;

internal sealed class RemoveRoleFromUserCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<RemoveRoleFromUserCommand>
{
    public async Task<Result> Handle(RemoveRoleFromUserCommand command, CancellationToken cancellationToken)
    {
        DbSet<UserRole> userRoles = unitOfWork.GetDbSet<UserRole>();
        DbSet<VohuManah.Domain.Library.User> users = unitOfWork.GetDbSet<VohuManah.Domain.Library.User>();

        VohuManah.Domain.Library.User? user = await users
            .FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure(VohuManah.Domain.Library.UserErrors.NotFound(command.UserId));
        }

        UserRole? userRole = await userRoles
            .FirstOrDefaultAsync(
                ur => ur.UserId == command.UserId && ur.RoleId == command.RoleId,
                cancellationToken);

        if (userRole is null)
        {
            return Result.Failure(Error.NotFound("UserRole.NotFound", "این نقش به کاربر اختصاص داده نشده است"));
        }

        userRoles.Remove(userRole);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

