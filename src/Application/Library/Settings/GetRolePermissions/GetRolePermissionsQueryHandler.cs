using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Users;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Settings.GetRolePermissions;

internal sealed class GetRolePermissionsQueryHandler(
    IApplicationDbContext dbContext) : IQueryHandler<GetRolePermissionsQuery, List<string>>
{
    public async Task<Result<List<string>>> Handle(GetRolePermissionsQuery query, CancellationToken cancellationToken)
    {
        DbSet<RolePermission> rolePermissions = dbContext.RolePermissions;
        DbSet<Role> roles = dbContext.Roles;

        Role? role = await roles.FirstOrDefaultAsync(r => r.Id == query.RoleId, cancellationToken);

        if (role is null)
        {
            return Result.Failure<List<string>>(Error.NotFound("Role.NotFound", "نقش یافت نشد"));
        }

        List<string> endpointNames = await rolePermissions
            .Where(rp => rp.RoleId == query.RoleId)
            .Select(rp => rp.EndpointName)
            .OrderBy(ep => ep)
            .ToListAsync(cancellationToken);

        return Result.Success(endpointNames);
    }
}

