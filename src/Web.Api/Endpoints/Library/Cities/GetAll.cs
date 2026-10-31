using Application.Abstractions.Messaging;
using Application.Library.Cities.GetAll;
using Application.Library.Settings;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Cities;

[RequireRole(EndpointNames.Cities.GetAll)]
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

