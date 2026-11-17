using Microsoft.AspNetCore.Authorization;

namespace VohuManah.Web.Api.Authorization;

public sealed class RoleEndpointRequirement : IAuthorizationRequirement
{
    public RoleEndpointRequirement(string endpointName)
    {
        EndpointName = endpointName;
    }

    public string EndpointName { get; }
}
