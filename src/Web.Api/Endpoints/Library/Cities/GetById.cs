using Application.Abstractions.Messaging;
using Application.Library.Cities.GetById;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Application.Library.Settings;

namespace Web.Api.Endpoints.Library.Cities;

[RequireRole(EndpointNames.Cities.GetById)]
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

