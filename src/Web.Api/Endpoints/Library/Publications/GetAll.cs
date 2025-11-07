using Application.Abstractions.Messaging;
using Application.Library.Publications.GetAll;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Publications;

internal sealed class GetAll : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/publications", async (
            IQueryHandler<GetAllPublicationsQuery, List<PublicationResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllPublicationsQuery();

            Result<List<PublicationResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags("Library.Publications");
    }
}

