using Application.Abstractions.Messaging;
using Application.Library.Cities.GetAll;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Cities;

[RequireRole("Library.Cities.GetAll")]
internal sealed class GetAll : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/cities", async (
            IQueryHandler<GetAllCitiesQuery, List<CityResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllCitiesQuery();

            Result<List<CityResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Cities");
    }
}

