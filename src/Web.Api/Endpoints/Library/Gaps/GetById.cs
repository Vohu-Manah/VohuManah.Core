using Application.Abstractions.Messaging;
using Application.Library.Gaps.GetById;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Gaps;

internal sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/gaps/{id:int}", async (
            int id,
            IQueryHandler<GetGapByIdQuery, GapResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetGapByIdQuery(id);

            Result<GapResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags("Library.Gaps");
    }
}

