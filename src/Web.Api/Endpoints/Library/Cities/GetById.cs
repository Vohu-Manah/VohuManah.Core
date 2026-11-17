using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Cities.GetById;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Library.Settings;

namespace VohuManah.Web.Api.Endpoints.Library.Cities;

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

