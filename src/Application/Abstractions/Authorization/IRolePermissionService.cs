namespace Application.Abstractions.Authorization;

public interface IRolePermissionService
{
    Task<bool> HasPermissionAsync(string roleName, string endpointName, CancellationToken cancellationToken = default);

    Task<List<string>> GetEndpointsForRoleAsync(string roleName, CancellationToken cancellationToken = default);

    Task<List<string>> GetRolesForEndpointAsync(string endpointName, CancellationToken cancellationToken = default);
}

