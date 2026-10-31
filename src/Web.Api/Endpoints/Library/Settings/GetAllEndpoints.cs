using Application.Abstractions.Messaging;
using Application.Library.Settings;
using Application.Library.Settings.GetAllEndpoints;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Settings;

[RequireRole(EndpointNames.Settings.GetAllEndpoints)]
internal sealed class GetAllEndpoints : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/settings/endpoints", async (
            IQueryHandler<GetAllEndpointsQuery, Dictionary<string, List<string>>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllEndpointsQuery();

            Result<Dictionary<string, List<string>>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Settings");
    }
}

