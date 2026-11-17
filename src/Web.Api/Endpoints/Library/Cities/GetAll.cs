using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Cities.GetAll;
using VohuManah.Application.Library.Settings;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;

namespace VohuManah.Web.Api.Endpoints.Library.Cities;

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

