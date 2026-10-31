using Application.Abstractions.Messaging;
using Application.Library.Settings;
using Application.Library.Visits.GetStats;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Visits;

[RequireRole(EndpointNames.Visits.GetStats)]
internal sealed class GetVisitStats : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/visits/stats", async (
            int? days,
            IQueryHandler<GetVisitStatsQuery, VisitStatsResponse> handler,
            CancellationToken cancellationToken) =>
        {
            Result<VisitStatsResponse> result = await handler.Handle(new GetVisitStatsQuery(days ?? 30), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Visits");
    }
}
