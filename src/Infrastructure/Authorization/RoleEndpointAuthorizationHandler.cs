using Application.Abstractions.Authorization;
using Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace Infrastructure.Authorization;

/// <summary>
/// Handler برای بررسی دسترسی نقش‌ها به endpointها از دیتابیس
/// </summary>
internal sealed class RoleEndpointAuthorizationHandler(IServiceScopeFactory serviceScopeFactory)
    : AuthorizationHandler<RoleEndpointRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        RoleEndpointRequirement requirement)
    {
        // بررسی احراز هویت
        if (context.User.Identity?.IsAuthenticated != true)
        {
            return;
        }

        using IServiceScope scope = serviceScopeFactory.CreateScope();
        IRolePermissionService rolePermissionService = scope.ServiceProvider.GetRequiredService<IRolePermissionService>();

        // گرفتن نقش‌های کاربر از claims
        var userRoles = context.User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        if (userRoles.Count == 0)
        {
            return; // کاربر هیچ نقشی ندارد
        }

        // بررسی دسترسی برای هر نقش
        foreach (var role in userRoles)
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

/// <summary>
/// Requirement برای بررسی دسترسی نقش به endpoint
/// </summary>
public sealed class RoleEndpointRequirement : IAuthorizationRequirement
{
    public RoleEndpointRequirement(string endpointName)
    {
        EndpointName = endpointName;
    }

    public string EndpointName { get; }
}

