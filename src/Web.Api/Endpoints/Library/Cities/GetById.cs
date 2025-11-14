using Application.Abstractions.Messaging;
using Application.Library.Cities.GetById;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Cities;

[RequireRole("Library.Cities.GetById")]
internal sealed class GetById : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/cities/{id:int}", async (
            int id,
            IQueryHandler<GetCityByIdQuery, CityResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetCityByIdQuery(id);

            Result<CityResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Cities");
    }
}

