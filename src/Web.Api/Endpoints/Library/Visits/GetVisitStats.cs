using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Settings;
using VohuManah.Application.Library.Visits.GetStats;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;

namespace VohuManah.Web.Api.Endpoints.Library.Visits;

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
