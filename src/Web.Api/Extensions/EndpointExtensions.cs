using Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;
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

        if (!string.IsNullOrEmpty(roleAttribute.EndpointName))
        {
            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new RoleEndpointRequirement(roleAttribute.EndpointName))
                .Build();

            return builder.RequireAuthorization(policy);
        }

#pragma warning disable CS0618 
        if (roleAttribute.SkipDatabaseCheck && roleAttribute.Roles.Length > 0)
        {
            return roleAttribute.RequireAll
                ? builder.RequireAllRoles(roleAttribute.Roles)
                : builder.RequireRole(roleAttribute.Roles);
        }
#pragma warning restore CS0618

        return builder;
    }

    public static RouteHandlerBuilder HasPermission(this RouteHandlerBuilder app, string permission)
    {
        return app.RequireAuthorization(permission);
    }

    public static RouteHandlerBuilder RequireRole(this RouteHandlerBuilder builder, params string[] roles)
    {
        var policy = new AuthorizationPolicyBuilder()
            .RequireRole(roles)
            .Build();

        return builder.RequireAuthorization(policy);
    }

    public static RouteHandlerBuilder RequireAllRoles(this RouteHandlerBuilder builder, params string[] roles)
    {
        var policyBuilder = new AuthorizationPolicyBuilder();

        foreach (var role in roles)
        {
            policyBuilder.RequireRole(role);
        }

        return builder.RequireAuthorization(policyBuilder.Build());
    }

    public static RouteHandlerBuilder ValidateRequestTypes<TRequest>(this RouteHandlerBuilder builder)
        where TRequest : class
    {
        return builder.AddEndpointFilter(async (context, next) =>
        {
            var requestParameter = context.Arguments.OfType<TRequest>().FirstOrDefault();

            if (requestParameter != null)
            {
                var validationError = Infrastructure.ModelBindingValidator.ValidateRequestTypes(requestParameter);
                if (validationError != null)
                {
                    return Results.Problem(
                        title: "خطای اعتبارسنجی",
                        detail: null,
                        type: "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                        statusCode: StatusCodes.Status400BadRequest,
                        extensions: new Dictionary<string, object?>
                        {
                            ["errors"] = validationError.Errors
                                .GroupBy(e => char.ToLowerInvariant(e.Code[0]) + e.Code.Substring(1))
                                .ToDictionary(
                                    g => g.Key,
                                    g => g.Select(e => e.Description).ToArray())
                        });
                }
            }

            return await next(context);
        });
    }
}
