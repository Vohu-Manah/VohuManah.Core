using Application.Abstractions.Messaging;
using Application.Library.PublicationTypes.GetAll;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.PublicationTypes;

internal sealed class GetAll : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/publication-types", async (
            IQueryHandler<GetAllPublicationTypesQuery, List<PublicationTypeResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllPublicationTypesQuery();

            Result<List<PublicationTypeResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags("Library.PublicationTypes");
    }
}

