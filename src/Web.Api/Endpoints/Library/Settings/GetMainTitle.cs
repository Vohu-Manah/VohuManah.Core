using Application.Abstractions.Messaging;
using Application.Library.Settings.GetMainTitle;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Settings;

internal sealed class GetMainTitle : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/settings/main-title", async (
            IQueryHandler<GetApplicationMainTitleQuery, string> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetApplicationMainTitleQuery();
            Result<string> result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags("Library.Settings");
    }
}


