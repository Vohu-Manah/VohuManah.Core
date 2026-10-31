using Application.Abstractions.Messaging;
using Application.Library.Publications.GetById;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Application.Library.Settings;

namespace Web.Api.Endpoints.Library.Publications;

[RequireRole(EndpointNames.Publications.GetById)]
internal sealed class GetById : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/publications/{id:int}", async (
            int id,
            IQueryHandler<GetPublicationByIdQuery, PublicationResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetPublicationByIdQuery(id);

            Result<PublicationResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Publications");
    }
}

