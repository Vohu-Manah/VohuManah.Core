using Application.Abstractions.Messaging;
using Application.Library.Publications.GetAllEntities;
using Domain.Library;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Publications;

[RequireRole("Library.Publications.GetAllEntities")]
internal sealed class GetAllEntities : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/publications/entities", async (
            IQueryHandler<GetAllPublicationEntitiesQuery, List<Publication>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllPublicationEntitiesQuery();

            Result<List<Publication>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Publications");
    }
}

