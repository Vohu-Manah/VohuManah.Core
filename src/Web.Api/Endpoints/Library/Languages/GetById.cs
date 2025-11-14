using Application.Abstractions.Messaging;
using Application.Library.Languages.GetById;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Languages;

[RequireRole("Library.Languages.GetById")]
internal sealed class GetById : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/languages/{id:int}", async (
            int id,
            IQueryHandler<GetLanguageByIdQuery, LanguageResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetLanguageByIdQuery(id);

            Result<LanguageResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Languages");
    }
}

