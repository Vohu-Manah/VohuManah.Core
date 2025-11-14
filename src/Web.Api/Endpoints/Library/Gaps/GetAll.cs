using Application.Abstractions.Messaging;
using Application.Library.Gaps.GetAll;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Gaps;

[RequireRole("Library.Gaps.GetAll")]
internal sealed class GetAll : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/gaps", async (
            IQueryHandler<GetAllGapsQuery, List<GapResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllGapsQuery();

            Result<List<GapResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Gaps");
    }
}

