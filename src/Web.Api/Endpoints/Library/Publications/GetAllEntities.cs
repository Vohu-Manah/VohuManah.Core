using Application.Abstractions.Messaging;
using Application.Library.Publications.GetAllEntities;
using Domain.Library;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Publications;

internal sealed class GetAllEntities : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/publications/entities", async (
            IQueryHandler<GetAllPublicationEntitiesQuery, List<Publication>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllPublicationEntitiesQuery();

            Result<List<Publication>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags("Library.Publications");
    }
}

