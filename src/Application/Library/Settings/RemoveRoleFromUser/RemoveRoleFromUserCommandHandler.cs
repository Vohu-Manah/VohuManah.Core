using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Users;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Settings.RemoveRoleFromUser;

internal sealed class RemoveRoleFromUserCommandHandler(
    IApplicationDbContext dbContext) : ICommandHandler<RemoveRoleFromUserCommand>
{
    public async Task<Result> Handle(RemoveRoleFromUserCommand command, CancellationToken cancellationToken)
    {
        DbSet<UserRole> userRoles = dbContext.UserRoles;
        DbSet<VohuManah.Domain.Users.User> users = dbContext.LibraryUsers;

        VohuManah.Domain.Users.User? user = await users
            .FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure(VohuManah.Domain.Users.UserErrors.NotFound(command.UserId));
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
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

