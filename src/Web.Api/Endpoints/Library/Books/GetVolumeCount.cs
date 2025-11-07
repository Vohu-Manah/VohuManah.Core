using Application.Abstractions.Messaging;
using Application.Library.Books.GetVolumeCount;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Books;

internal sealed class GetVolumeCount : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/books/volume-count", async (
            IQueryHandler<GetBookVolumeCountQuery, int> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetBookVolumeCountQuery();

            Result<int> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags("Library.Books");
    }
}

