using Application.Abstractions.Messaging;
using Application.Library.Manuscripts.GetCountByYear;
using Application.Library._Shared;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Manuscripts;

[RequireRole("Library.Manuscripts.GetCountByYear")]
internal sealed class GetCountByYear : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/manuscripts/count-by-year", async (
            IQueryHandler<GetManuscriptCountByYearQuery, List<ListItemResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetManuscriptCountByYearQuery();

            Result<List<ListItemResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Manuscripts");
    }
}

