using Application.Abstractions.Messaging;
using Application.Library.Publications.GetCount;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Publications;

internal sealed class GetCount : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/publications/count", async (
            IQueryHandler<GetPublicationCountQuery, int> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetPublicationCountQuery();

            Result<int> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags("Library.Publications");
    }
}

