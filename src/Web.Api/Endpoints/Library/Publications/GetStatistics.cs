using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Publications.GetStatistics;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Publications;

[RequireRole(PermissionNames.Publications.GetStatistics)]
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

