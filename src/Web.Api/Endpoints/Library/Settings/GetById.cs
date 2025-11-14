using Application.Abstractions.Messaging;
using Application.Library.Settings.GetById;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Settings;

[RequireRole("Library.Settings.GetById")]
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

