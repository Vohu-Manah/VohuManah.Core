using Application.Abstractions.Messaging;
using Application.Library.Settings.GetByName;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Settings;

[RequireRole("Library.Settings.GetByName")]
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

