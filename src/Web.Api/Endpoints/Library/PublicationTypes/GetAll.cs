using Application.Abstractions.Messaging;
using Application.Library.PublicationTypes.GetAll;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Application.Library.Settings;

namespace Web.Api.Endpoints.Library.PublicationTypes;

[RequireRole(EndpointNames.PublicationTypes.GetAll)]
internal sealed class GetAll : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/publication-types", async (
            IQueryHandler<GetAllPublicationTypesQuery, List<PublicationTypeResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllPublicationTypesQuery();

            Result<List<PublicationTypeResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.PublicationTypes");
    }
}

