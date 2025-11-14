using System.Reflection;
using Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;

namespace Web.Api.Extensions;

public static class EndpointExtensions
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services, Assembly assembly)
    {
        ServiceDescriptor[] serviceDescriptors = assembly
            .DefinedTypes
            .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                           type.IsAssignableTo(typeof(IEndpoint)))
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
            .ToArray();

        services.TryAddEnumerable(serviceDescriptors);

        return services;
    }

    public static IApplicationBuilder MapEndpoints(
        this WebApplication app,
        RouteGroupBuilder? routeGroupBuilder = null)
    {
        IEnumerable<IEndpoint> endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();

        IEndpointRouteBuilder builder = routeGroupBuilder is null ? app : routeGroupBuilder;

        foreach (IEndpoint endpoint in endpoints)
        {
            endpoint.MapEndpoint(builder);
        }

        return app;
    }

    /// <summary>
    /// اعمال خودکار نقش‌ها از RequireRoleAttribute روی کلاس endpoint
    /// این method از دیتابیس برای بررسی دسترسی استفاده می‌کند
    /// </summary>
    public static RouteHandlerBuilder ApplyRoleAuthorization(
        this RouteHandlerBuilder builder,
        IEndpoint endpoint)
    {
        Type endpointType = endpoint.GetType();
        RequireRoleAttribute? roleAttribute = endpointType.GetCustomAttribute<RequireRoleAttribute>();

        if (roleAttribute is null)
        {
            return builder;
        }

        // استفاده از دیتابیس برای بررسی دسترسی
        if (!string.IsNullOrEmpty(roleAttribute.EndpointName))
        {
            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new RoleEndpointRequirement(roleAttribute.EndpointName))
                .Build();

            return builder.RequireAuthorization(policy);
        }

        // اگر SkipDatabaseCheck true باشد، از روش قدیمی استفاده می‌کنیم (backward compatibility)
#pragma warning disable CS0618 // Type or member is obsolete
        if (roleAttribute.SkipDatabaseCheck && roleAttribute.Roles.Length > 0)
        {
            return roleAttribute.RequireAll
                ? builder.RequireAllRoles(roleAttribute.Roles)
                : builder.RequireRole(roleAttribute.Roles);
        }
#pragma warning restore CS0618 // Type or member is obsolete

        return builder;
    }

    public static RouteHandlerBuilder HasPermission(this RouteHandlerBuilder app, string permission)
    {
        return app.RequireAuthorization(permission);
    }

    /// <summary>
    /// Requires the user to have at least one of the specified roles.
    /// Roles are checked from JWT token claims (ClaimTypes.Role).
    /// </summary>
    public static RouteHandlerBuilder RequireRole(this RouteHandlerBuilder builder, params string[] roles)
    {
        var policy = new AuthorizationPolicyBuilder()
            .RequireRole(roles)
            .Build();
        
        return builder.RequireAuthorization(policy);
    }

    /// <summary>
    /// Requires the user to have all of the specified roles.
    /// Roles are checked from JWT token claims (ClaimTypes.Role).
    /// </summary>
    public static RouteHandlerBuilder RequireAllRoles(this RouteHandlerBuilder builder, params string[] roles)
    {
        var policyBuilder = new AuthorizationPolicyBuilder();
        
        foreach (var role in roles)
        {
            policyBuilder.RequireRole(role);
        }
        
        return builder.RequireAuthorization(policyBuilder.Build());
    }
}
