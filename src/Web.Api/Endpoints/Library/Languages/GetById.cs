using Application.Abstractions.Messaging;
using Application.Library.Languages.GetById;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Languages;

internal sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
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
        .WithTags("Library.Languages");
    }
}

