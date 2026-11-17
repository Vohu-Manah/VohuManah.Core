using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Settings.GetById;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Library.Settings;

namespace VohuManah.Web.Api.Endpoints.Library.Settings;

[RequireRole(EndpointNames.Settings.GetById)]
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

