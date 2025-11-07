using Application.Abstractions.Messaging;
using Application.Library.Cities.GetAll;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Cities;

internal sealed class GetAll : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/cities", async (
            IQueryHandler<GetAllCitiesQuery, List<CityResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllCitiesQuery();

            Result<List<CityResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags("Library.Cities");
    }
}

