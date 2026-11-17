using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Users;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Settings.GetUserRoles;

internal sealed class GetUserRolesQueryHandler(
    IApplicationDbContext dbContext) : IQueryHandler<GetUserRolesQuery, List<UserRoleResponse>>
{
    public async Task<Result<List<UserRoleResponse>>> Handle(GetUserRolesQuery query, CancellationToken cancellationToken)
    {
        DbSet<UserRole> userRoles = dbContext.UserRoles;
        DbSet<RolePermission> rolePermissions = dbContext.RolePermissions;
        DbSet<VohuManah.Domain.Users.User> users = dbContext.LibraryUsers;

        VohuManah.Domain.Users.User? user = await users
            .FirstOrDefaultAsync(u => u.Id == query.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<List<UserRoleResponse>>(VohuManah.Domain.Users.UserErrors.NotFound(query.UserId));
        }

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

