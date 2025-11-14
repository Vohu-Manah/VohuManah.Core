using Application.Abstractions.Messaging;
using Application.Library.Manuscripts.GetAll;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Manuscripts;

[RequireRole("Library.Manuscripts.GetAll")]
internal sealed class GetAll : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/manuscripts", async (
            IQueryHandler<GetAllManuscriptsQuery, List<ManuscriptResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllManuscriptsQuery();

            Result<List<ManuscriptResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Manuscripts");
    }
}

