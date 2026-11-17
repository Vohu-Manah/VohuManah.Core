using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Settings.GetAllRoles;
using VohuManah.Domain.Users;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Settings.CreateRole;

internal sealed class CreateRoleCommandHandler(
    IApplicationDbContext dbContext,
    ICacheManager cacheManager) : ICommandHandler<CreateRoleCommand, RoleResponse>
{

    public async Task<Result<RoleResponse>> Handle(CreateRoleCommand command, CancellationToken cancellationToken)
    {
        DbSet<Role> roles = dbContext.Roles;

        string normalizedName = command.Name.Trim();

        bool exists = await roles.AnyAsync(
            r => r.Name.Equals(normalizedName, StringComparison.OrdinalIgnoreCase),
            cancellationToken).ConfigureAwait(false);

        if (exists)
        {
            return Result.Failure<RoleResponse>(Error.Conflict("Role.Exists", "این نقش قبلاً ایجاد شده است"));
        }

        var role = new Role
        {
            Name = normalizedName
        };

        roles.Add(role);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        if (RolePermissionDefaults.TryGetDefaults(role.Name, out var defaultEndpoints) && defaultEndpoints.Length > 0)
        {
            DbSet<RolePermission> rolePermissions = dbContext.RolePermissions;

            foreach (string endpoint in defaultEndpoints)
            {
                rolePermissions.Add(new RolePermission
                {
                    RoleId = role.Id,
                    EndpointName = endpoint
                });
            }

            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        cacheManager.Remove(GetAllRolesQueryHandler.RolesAllKey);

        return Result.Success(new RoleResponse
        {
            Id = role.Id,
            Name = role.Name
        });
    }
}

