using System.Threading;
using Application.Abstractions.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace Infrastructure.Authorization;

internal sealed class RoleEndpointAuthorizationHandler(IServiceScopeFactory serviceScopeFactory)
    : AuthorizationHandler<RoleEndpointRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        RoleEndpointRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            return;
        }

        using IServiceScope scope = serviceScopeFactory.CreateScope();
        IRolePermissionService rolePermissionService = scope.ServiceProvider.GetRequiredService<IRolePermissionService>();

        var userRoles = context.User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        if (userRoles.Count == 0)
        {
            return;
        }

        foreach (string? role in userRoles)
        {
            bool hasPermission = await rolePermissionService.HasPermissionAsync(
                role,
                requirement.EndpointName,
                CancellationToken.None);

            if (hasPermission)
            {
                context.Succeed(requirement);
                return;
            }
        }
    }
}

public sealed class RoleEndpointRequirement : IAuthorizationRequirement
{
    public RoleEndpointRequirement(string endpointName)
    {
        EndpointName = endpointName;
    }

    public string EndpointName { get; }
}

