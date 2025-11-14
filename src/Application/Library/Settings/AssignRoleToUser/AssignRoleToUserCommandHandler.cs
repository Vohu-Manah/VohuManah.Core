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
        DbSet<UserRole> userRoles = unitOfWork.Set<UserRole>();
        DbSet<Role> roles = unitOfWork.Set<Role>();
        DbSet<Domain.Library.User> users = unitOfWork.Set<Domain.Library.User>();

        // بررسی وجود کاربر
        bool userExists = await users.AnyAsync(u => u.UserName == command.UserName, cancellationToken);
        if (!userExists)
        {
            return Result.Failure(Domain.Library.UserErrors.NotFound(command.UserName));
        }

        // بررسی وجود نقش
        bool roleExists = await roles.AnyAsync(r => r.Id == command.RoleId, cancellationToken);
        if (!roleExists)
        {
            return Result.Failure(Error.NotFound("Role.NotFound", "نقش یافت نشد"));
        }

        // بررسی اینکه آیا این نقش قبلاً به کاربر اختصاص داده شده است
        bool alreadyAssigned = await userRoles
            .AnyAsync(ur => ur.UserName == command.UserName && ur.RoleId == command.RoleId, cancellationToken);
        
        if (alreadyAssigned)
        {
            return Result.Failure(Error.Conflict("UserRole.AlreadyExists", "این نقش قبلاً به کاربر اختصاص داده شده است"));
        }

        // اختصاص نقش به کاربر
        var userRole = new UserRole
        {
            UserName = command.UserName,
            RoleId = command.RoleId
        };

        userRoles.Add(userRole);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

