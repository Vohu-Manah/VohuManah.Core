using Application.Abstractions.Messaging;
using Application.Library.Manuscripts.GetCount;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Manuscripts;

internal sealed class GetCount : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/manuscripts/count", async (
            IQueryHandler<GetManuscriptCountQuery, int> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetManuscriptCountQuery();

            Result<int> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags("Library.Manuscripts");
    }
}

