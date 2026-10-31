using Application.Abstractions.Messaging;
using Application.Library.PublicationTypes.GetById;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Application.Library.Settings;

namespace Web.Api.Endpoints.Library.PublicationTypes;

[RequireRole(EndpointNames.PublicationTypes.GetById)]
internal sealed class GetById : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/publication-types/{id:int}", async (
            int id,
            IQueryHandler<GetPublicationTypeByIdQuery, PublicationTypeResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetPublicationTypeByIdQuery(id);

            Result<PublicationTypeResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.PublicationTypes");
    }
}

