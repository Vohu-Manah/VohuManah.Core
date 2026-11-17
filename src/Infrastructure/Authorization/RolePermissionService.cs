using System.Threading;
using VohuManah.Application.Abstractions.Authorization;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Domain.Library;
using Microsoft.EntityFrameworkCore;

namespace VohuManah.Infrastructure.Authorization;

internal sealed class RoleEndpointService(IUnitOfWork unitOfWork) : IRolePermissionService
{
    public async Task<bool> HasPermissionAsync(string roleName, string endpointName, CancellationToken cancellationToken = default)
    {
        var hasPermission = await unitOfWork.GetDbSet<RolePermission>()
            .Include(rp => rp.Role)
            .AnyAsync(
                rp => rp.Role!.Name == roleName && rp.EndpointName == endpointName,
                cancellationToken);

        return hasPermission;
    }

    public async Task<List<string>> GetEndpointsForRoleAsync(string roleName, CancellationToken cancellationToken = default)
    {
        var endpoints = await unitOfWork.GetDbSet<RolePermission>()
            .Include(rp => rp.Role)
            .Where(rp => rp.Role!.Name == roleName)
            .Select(rp => rp.EndpointName)
            .ToListAsync(cancellationToken);

        return endpoints;
    }

    public async Task<List<string>> GetRolesForEndpointAsync(string endpointName, CancellationToken cancellationToken = default)
    {
        var roles = await unitOfWork.GetDbSet<RolePermission>()
            .Include(rp => rp.Role)
            .Where(rp => rp.EndpointName == endpointName)
            .Select(rp => rp.Role!.Name)
            .ToListAsync(cancellationToken);

        return roles;
    }
}

