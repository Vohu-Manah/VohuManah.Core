using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Settings.GetUserRoles;

internal sealed class GetUserRolesQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetUserRolesQuery, List<UserRoleResponse>>
{
    public async Task<Result<List<UserRoleResponse>>> Handle(GetUserRolesQuery query, CancellationToken cancellationToken)
    {
        DbSet<UserRole> userRoles = unitOfWork.Set<UserRole>();
        DbSet<RolePermission> rolePermissions = unitOfWork.Set<RolePermission>();
        DbSet<Domain.Library.User> users = unitOfWork.Set<Domain.Library.User>();

        Domain.Library.User? user = await users
            .FirstOrDefaultAsync(u => u.Id == query.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<List<UserRoleResponse>>(Domain.Library.UserErrors.NotFound(query.UserId));
        }

        // گرفتن تمام endpoint permissions کاربر از طریق نقش‌هایش
        var endpointPermissions = await userRoles
            .Where(ur => ur.UserId == query.UserId)
            .Join(
                rolePermissions,
                ur => ur.RoleId,
                rp => rp.RoleId,
                (ur, rp) => rp.EndpointName)
            .Distinct()
            .OrderBy(ep => ep)
            .Select(ep => new UserRoleResponse
            {
                EndpointName = ep
            })
            .ToListAsync(cancellationToken);

        return Result.Success(endpointPermissions);
    }
}

