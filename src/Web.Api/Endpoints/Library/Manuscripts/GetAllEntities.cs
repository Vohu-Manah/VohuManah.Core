using Application.Abstractions.Messaging;
using Application.Library.Manuscripts.GetAllEntities;
using Domain.Library;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Manuscripts;

internal sealed class GetAllEntities : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/manuscripts/entities", async (
            IQueryHandler<GetAllManuscriptEntitiesQuery, List<Manuscript>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllManuscriptEntitiesQuery();

            Result<List<Manuscript>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags("Library.Manuscripts");
    }
}

