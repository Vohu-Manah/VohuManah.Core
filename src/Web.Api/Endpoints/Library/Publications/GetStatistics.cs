using Application.Abstractions.Messaging;
using Application.Library.Publications.GetStatistics;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Application.Library.Settings;

namespace Web.Api.Endpoints.Library.Publications;

[RequireRole(EndpointNames.Publications.GetStatistics)]
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

