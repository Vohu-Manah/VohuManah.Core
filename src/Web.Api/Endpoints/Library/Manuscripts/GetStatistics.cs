using Application.Abstractions.Messaging;
using Application.Library.Manuscripts.GetStatistics;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Manuscripts;

[RequireRole("Library.Manuscripts.GetStatistics")]
internal sealed class GetStatistics : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/manuscripts/statistics", async (
            IQueryHandler<GetManuscriptStatisticsQuery, ManuscriptStatisticsResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetManuscriptStatisticsQuery();

            Result<ManuscriptStatisticsResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Manuscripts");
    }
}

