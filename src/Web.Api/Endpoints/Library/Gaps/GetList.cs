using Application.Abstractions.Messaging;
using Application.Library.Gaps.GetList;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Gaps;

internal sealed class GetList : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/gaps/list", async (
            IQueryHandler<GetGapListQuery, List<GapListResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetGapListQuery();

            Result<List<GapListResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags("Library.Gaps");
    }
}

