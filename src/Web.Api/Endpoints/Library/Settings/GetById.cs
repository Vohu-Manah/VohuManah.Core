using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Settings.GetById;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Http;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Settings;

[RequireRole(PermissionNames.Settings.GetById)]
internal sealed class GetById : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/settings/{id:int}", async (
            int id,
            IQueryHandler<GetSettingsByIdQuery, SettingsResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetSettingsByIdQuery(id);

            Result<SettingsResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Settings");
    }
}

