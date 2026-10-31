using Application.Abstractions.Messaging;
using Application.Library.Settings.GetSystemInfo;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Application.Library.Settings;

namespace Web.Api.Endpoints.Library.Settings;

[RequireRole(EndpointNames.Settings.GetSystemInfo)]
internal sealed class GetSystemInfoEndpoint : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/settings/system-info", async (
            IQueryHandler<GetSystemInfoQuery, SystemInfoResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetSystemInfoQuery();
            Result<SystemInfoResponse> result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Settings");
    }
}


