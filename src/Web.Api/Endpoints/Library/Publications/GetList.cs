using Application.Abstractions.Messaging;
using Application.Library.Publications.GetList;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Publications;

internal sealed class GetList : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/publications/list", async (
            IQueryHandler<GetPublicationListQuery, List<PublicationListResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetPublicationListQuery();

            Result<List<PublicationListResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags("Library.Publications");
    }
}

