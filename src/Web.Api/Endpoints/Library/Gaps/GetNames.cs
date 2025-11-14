using Application.Abstractions.Messaging;
using Application.Library.Gaps.GetNames;
using Application.Library._Shared;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Gaps;

[RequireRole("Library.Gaps.GetNames")]
internal sealed class GetNames : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/gaps/names", async (
            bool? addAll,
            IQueryHandler<GetGapNamesQuery, List<SelectItemResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetGapNamesQuery(addAll ?? false);
            Result<List<SelectItemResponse>> result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Gaps");
    }
}


