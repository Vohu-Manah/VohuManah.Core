using Application.Abstractions.Messaging;
using Application.Library.Settings.GetAppInfo;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Application.Library.Settings;

namespace Web.Api.Endpoints.Library.Settings;

[RequireRole(EndpointNames.Settings.GetAppInfo)]
internal sealed class GetAppInfoEndpoint : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/settings/app-info", async (
            IQueryHandler<GetAppInfoQuery, AppInfoResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAppInfoQuery();
            Result<AppInfoResponse> result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Settings");
    }
}


