using Application.Abstractions.Messaging;
using Application.Library.Languages.GetList;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Languages;

internal sealed class GetList : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/languages/list", async (
            IQueryHandler<GetLanguageListQuery, List<LanguageListResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetLanguageListQuery();

            Result<List<LanguageListResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags("Library.Languages");
    }
}

