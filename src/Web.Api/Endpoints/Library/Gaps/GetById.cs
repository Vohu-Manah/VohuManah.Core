using Application.Abstractions.Messaging;
using Application.Library.Gaps.GetById;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Application.Library.Settings;

namespace Web.Api.Endpoints.Library.Gaps;

[RequireRole(EndpointNames.Gaps.GetById)]
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

