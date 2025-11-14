using Application.Abstractions.Messaging;
using Application.Library.Settings.GetAll;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Settings;

[RequireRole("Library.Settings.GetAll")]
internal sealed class GetAll : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/settings", async (
            IQueryHandler<GetAllSettingsQuery, List<SettingsResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllSettingsQuery();

            Result<List<SettingsResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Settings");
    }
}

