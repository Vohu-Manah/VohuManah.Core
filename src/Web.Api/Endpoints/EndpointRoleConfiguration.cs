using Application.Library.Settings;
using System.Collections.Immutable;

namespace Web.Api.Endpoints;

public static class EndpointRoleConfiguration
{

    public const string DefaultRole = "Admin";


    public static readonly ImmutableHashSet<string> AnonymousEndpoints = ImmutableHashSet.Create(
        "Login",
        "RefreshToken");

    public static readonly ImmutableDictionary<string, ImmutableArray<string>> RoleEndpointMapping =
        RolePermissionDefaults.RoleEndpointMapping;

    public static bool IsAnonymous(string endpointName)
    {
        return AnonymousEndpoints.Contains(endpointName);
    }

    public static string[] GetRolesForEndpoint(string endpointFullName)
    {
        foreach (KeyValuePair<string, ImmutableArray<string>> mapping in RoleEndpointMapping)
        {
            if (mapping.Value.Contains(endpointFullName))
            {
                return new[] { mapping.Key };
            }
        }
        return new[] { DefaultRole };
    }
}

