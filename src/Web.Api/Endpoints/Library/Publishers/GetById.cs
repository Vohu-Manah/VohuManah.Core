using Application.Abstractions.Messaging;
using Application.Library.Publishers.GetById;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Publishers;

internal sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/publishers/{id:int}", async (
            int id,
            IQueryHandler<GetPublisherByIdQuery, PublisherResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetPublisherByIdQuery(id);

            Result<PublisherResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags("Library.Publishers");
    }
}

