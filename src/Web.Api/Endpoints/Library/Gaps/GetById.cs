using Application.Abstractions.Messaging;
using Application.Library.Gaps.GetById;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Gaps;

[RequireRole("Library.Gaps.GetById")]
internal sealed class GetById : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
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
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Gaps");
    }
}

