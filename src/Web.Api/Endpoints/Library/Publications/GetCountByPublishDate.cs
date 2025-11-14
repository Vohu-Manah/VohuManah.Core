using Application.Abstractions.Messaging;
using Application.Library.Publications.GetCountByPublishDate;
using Application.Library._Shared;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Publications;

[RequireRole("Library.Publications.GetCountByPublishDate")]
internal sealed class GetCountByPublishDate : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/publications/count-by-publish-date", async (
            IQueryHandler<GetPublicationCountByPublishDateQuery, List<ListItemResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetPublicationCountByPublishDateQuery();

            Result<List<ListItemResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Publications");
    }
}

