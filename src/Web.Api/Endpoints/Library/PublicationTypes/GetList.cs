using Application.Abstractions.Messaging;
using Application.Library.PublicationTypes.GetList;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.PublicationTypes;

internal sealed class GetList : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/publicationtypes/list", async (
            IQueryHandler<GetPublicationTypeListQuery, List<PublicationTypeListResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetPublicationTypeListQuery();

            Result<List<PublicationTypeListResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags("Library.PublicationTypes");
    }
}

