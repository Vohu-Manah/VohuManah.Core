using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Settings.RemoveRoleFromUser;

internal sealed class RemoveRoleFromUserCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<RemoveRoleFromUserCommand>
{
    public async Task<Result> Handle(RemoveRoleFromUserCommand command, CancellationToken cancellationToken)
    {
        DbSet<UserRole> userRoles = unitOfWork.Set<UserRole>();
        DbSet<Domain.Library.User> users = unitOfWork.Set<Domain.Library.User>();

        Domain.Library.User? user = await users
            .FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure(Domain.Library.UserErrors.NotFound(command.UserId));
        }

        // پیدا کردن UserRole
        UserRole? userRole = await userRoles
            .FirstOrDefaultAsync(
                ur => ur.UserId == command.UserId && ur.RoleId == command.RoleId,
                cancellationToken);

        if (userRole is null)
        {
            return Result.Failure(Error.NotFound("UserRole.NotFound", "این نقش به کاربر اختصاص داده نشده است"));
        }

        // حذف نقش از کاربر
        userRoles.Remove(userRole);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

