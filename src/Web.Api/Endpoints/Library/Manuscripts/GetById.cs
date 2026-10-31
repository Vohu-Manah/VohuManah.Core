using Application.Abstractions.Messaging;
using Application.Library.Manuscripts.GetById;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Application.Library.Settings;

namespace Web.Api.Endpoints.Library.Manuscripts;

[RequireRole(EndpointNames.Manuscripts.GetById)]
internal sealed class GetById : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/manuscripts/{id:int}", async (
            int id,
            IQueryHandler<GetManuscriptByIdQuery, ManuscriptResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetManuscriptByIdQuery(id);

            Result<ManuscriptResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Manuscripts");
    }
}

