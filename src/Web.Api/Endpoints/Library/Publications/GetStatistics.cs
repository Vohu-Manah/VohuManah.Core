using Application.Abstractions.Messaging;
using Application.Library.Publications.GetStatistics;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Publications;

[RequireRole("Library.Publications.GetStatistics")]
internal sealed class GetStatistics : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/publications/statistics", async (
            IQueryHandler<GetPublicationStatisticsQuery, PublicationStatisticsResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetPublicationStatisticsQuery();

            Result<PublicationStatisticsResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Publications");
    }
}

