using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Users;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Settings.AssignRoleToUser;

internal sealed class AssignRoleToUserCommandHandler(
    IApplicationDbContext dbContext) : ICommandHandler<AssignRoleToUserCommand>
{
    public async Task<Result> Handle(AssignRoleToUserCommand command, CancellationToken cancellationToken)
    {
        DbSet<UserRole> userRoles = dbContext.UserRoles;
        DbSet<Role> roles = dbContext.Roles;
        DbSet<VohuManah.Domain.Users.User> users = dbContext.LibraryUsers;

        VohuManah.Domain.Users.User? user = await users
            .FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure(VohuManah.Domain.Users.UserErrors.NotFound(command.UserId));
        }
        bool roleExists = await roles.AnyAsync(r => r.Id == command.RoleId, cancellationToken);
        if (!roleExists)
        {
            return Result.Failure(Error.NotFound("Role.NotFound", "نقش یافت نشد"));
        }

        bool alreadyAssigned = await userRoles
            .AnyAsync(
                ur => ur.UserId == command.UserId && ur.RoleId == command.RoleId,
                cancellationToken);

        if (alreadyAssigned)
        {
            return Result.Failure(Error.Conflict("UserRole.AlreadyExists", "این نقش قبلاً به کاربر اختصاص داده شده است"));
        }

        var userRole = new UserRole
        {
            UserId = command.UserId,
            UserName = user.UserName,
            RoleId = command.RoleId
        };

        userRoles.Add(userRole);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

