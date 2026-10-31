using Application.Abstractions.Messaging;
using Application.Library.Gaps.GetList;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Application.Library.Settings;

namespace Web.Api.Endpoints.Library.Gaps;

[RequireRole(EndpointNames.Gaps.GetList)]
internal sealed class GetList : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/gaps/list", async (
            IQueryHandler<GetGapListQuery, List<GapListResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetGapListQuery();

            Result<List<GapListResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Gaps");
    }
}

