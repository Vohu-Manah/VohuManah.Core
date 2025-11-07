using Application.Abstractions.Messaging;
using Application.Library.PublicationTypes.GetById;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.PublicationTypes;

internal sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/publication-types/{id:int}", async (
            int id,
            IQueryHandler<GetPublicationTypeByIdQuery, PublicationTypeResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetPublicationTypeByIdQuery(id);

            Result<PublicationTypeResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags("Library.PublicationTypes");
    }
}

