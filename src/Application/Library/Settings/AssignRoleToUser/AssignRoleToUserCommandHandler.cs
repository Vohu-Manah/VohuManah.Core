using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Library;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Settings.AssignRoleToUser;

internal sealed class AssignRoleToUserCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<AssignRoleToUserCommand>
{
    public async Task<Result> Handle(AssignRoleToUserCommand command, CancellationToken cancellationToken)
    {
        DbSet<UserRole> userRoles = unitOfWork.GetDbSet<UserRole>();
        DbSet<Role> roles = unitOfWork.GetDbSet<Role>();
        DbSet<VohuManah.Domain.Library.User> users = unitOfWork.GetDbSet<VohuManah.Domain.Library.User>();

        VohuManah.Domain.Library.User? user = await users
            .FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure(VohuManah.Domain.Library.UserErrors.NotFound(command.UserId));
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
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

