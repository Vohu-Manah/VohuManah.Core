using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Settings.GetSystemInfo;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Library.Settings;

namespace VohuManah.Web.Api.Endpoints.Library.Settings;

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


