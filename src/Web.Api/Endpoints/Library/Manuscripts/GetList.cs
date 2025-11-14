using Application.Abstractions.Messaging;
using Application.Library.Manuscripts.GetList;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Manuscripts;

[RequireRole("Library.Manuscripts.GetList")]
internal sealed class GetList : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/manuscripts/list", async (
            IQueryHandler<GetManuscriptListQuery, List<ManuscriptListResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetManuscriptListQuery();

            Result<List<ManuscriptListResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Manuscripts");
    }
}

