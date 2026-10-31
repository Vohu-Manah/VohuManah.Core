using Application.Abstractions.Authorization;
using Application.Abstractions.Data;
using Domain.Library;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Authorization;

internal sealed class RoleEndpointService(IUnitOfWork unitOfWork) : IRolePermissionService
{
    public async Task<bool> HasPermissionAsync(string roleName, string endpointName, CancellationToken cancellationToken = default)
    {
        var hasPermission = await unitOfWork.GetDbSet<RoleEndpoint>()
            .Include(rp => rp.Role)
            .AnyAsync(
                rp => rp.Role!.Name == roleName && rp.EndpointName == endpointName,
                cancellationToken);

        return hasPermission;
    }

    public async Task<List<string>> GetEndpointsForRoleAsync(string roleName, CancellationToken cancellationToken = default)
    {
        var endpoints = await unitOfWork.GetDbSet<RoleEndpoint>()
            .Include(rp => rp.Role)
            .Where(rp => rp.Role!.Name == roleName)
            .Select(rp => rp.EndpointName)
            .ToListAsync(cancellationToken);

        return endpoints;
    }

    public async Task<List<string>> GetRolesForEndpointAsync(string endpointName, CancellationToken cancellationToken = default)
    {
        var roles = await unitOfWork.GetDbSet<RoleEndpoint>()
            .Include(rp => rp.Role)
            .Where(rp => rp.EndpointName == endpointName)
            .Select(rp => rp.Role!.Name)
            .ToListAsync(cancellationToken);

        return roles;
    }
}

