using Application.Abstractions.Messaging;
using Application.Library.Languages.GetAll;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Languages;

internal sealed class GetAll : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/languages", async (
            IQueryHandler<GetAllLanguagesQuery, List<LanguageResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllLanguagesQuery();

            Result<List<LanguageResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags("Library.Languages");
    }
}

