using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Cities.GetList;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Cities;

[RequireRole(PermissionNames.Cities.GetList)]
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

