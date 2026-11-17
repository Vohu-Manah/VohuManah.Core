using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Manuscripts.GetStatistics;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Manuscripts;

[RequireRole(PermissionNames.Manuscripts.GetStatistics)]
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

