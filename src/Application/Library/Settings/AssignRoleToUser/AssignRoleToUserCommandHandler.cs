using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Settings.AssignRoleToUser;

internal sealed class AssignRoleToUserCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<AssignRoleToUserCommand>
{
    public async Task<Result> Handle(AssignRoleToUserCommand command, CancellationToken cancellationToken)
    {
        DbSet<UserRole> userRoles = unitOfWork.GetDbSet<UserRole>();
        DbSet<Role> roles = unitOfWork.GetDbSet<Role>();
        DbSet<Domain.Library.User> users = unitOfWork.GetDbSet<Domain.Library.User>();

        Domain.Library.User? user = await users
            .FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure(Domain.Library.UserErrors.NotFound(command.UserId));
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

