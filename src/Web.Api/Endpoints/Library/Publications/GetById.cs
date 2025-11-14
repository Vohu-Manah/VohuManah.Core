using Application.Abstractions.Messaging;
using Application.Library.Publications.GetById;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Publications;

[RequireRole("Library.Publications.GetById")]
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

