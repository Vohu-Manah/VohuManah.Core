using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Settings.GetAppInfo;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Http;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Settings;

[RequireRole(PermissionNames.Settings.GetAppInfo)]
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


