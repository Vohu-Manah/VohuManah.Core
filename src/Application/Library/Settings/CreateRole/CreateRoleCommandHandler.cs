using Application.Abstractions.Caching;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Library.Settings.GetAllRoles;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Settings.CreateRole;

internal sealed class CreateRoleCommandHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : ICommandHandler<CreateRoleCommand, RoleResponse>
{

    public async Task<Result<RoleResponse>> Handle(CreateRoleCommand command, CancellationToken cancellationToken)
    {
        DbSet<Role> roles = unitOfWork.GetDbSet<Role>();

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
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        if (RolePermissionDefaults.TryGetDefaults(role.Name, out var defaultEndpoints) && defaultEndpoints.Length > 0)
        {
            DbSet<RoleEndpoint> rolePermissions = unitOfWork.GetDbSet<RoleEndpoint>();

            foreach (string endpoint in defaultEndpoints)
            {
                rolePermissions.Add(new RoleEndpoint
                {
                    RoleId = role.Id,
                    EndpointName = endpoint
                });
            }

            await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        cacheManager.Remove(GetAllRolesQueryHandler.RolesAllKey);

        return Result.Success(new RoleResponse
        {
            Id = role.Id,
            Name = role.Name
        });
    }
}

