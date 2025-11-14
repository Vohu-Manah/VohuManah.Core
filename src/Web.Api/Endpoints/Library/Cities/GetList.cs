using Application.Abstractions.Messaging;
using Application.Library.Cities.GetList;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Cities;

[RequireRole("Library.Cities.GetList")]
internal sealed class GetList : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/cities/list", async (
            IQueryHandler<GetCityListQuery, List<CityListResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetCityListQuery();

            Result<List<CityListResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Cities");
    }
}

