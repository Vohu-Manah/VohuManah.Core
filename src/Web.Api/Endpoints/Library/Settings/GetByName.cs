using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Settings.GetByName;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Settings;

[RequireRole(PermissionNames.Settings.GetByName)]
internal sealed class GetByName : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/settings/by-name/{name}", async (
            string name,
            IQueryHandler<GetSettingsByNameQuery, SettingsResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetSettingsByNameQuery(name);

            Result<SettingsResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Settings");
    }
}

