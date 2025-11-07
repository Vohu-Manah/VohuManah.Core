using Application.Abstractions.Messaging;
using Application.Library.Settings.GetBackground;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Settings;

internal sealed class GetBackground : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/settings/background/folder", async (
            IQueryHandler<GetCurrentBackgroundImageFolderQuery, string> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetCurrentBackgroundImageFolderQuery();
            Result<string> result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags("Library.Settings");

        app.MapGet("library/settings/background/name", async (
            IQueryHandler<GetCurrentBackgroundImageNameQuery, string> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetCurrentBackgroundImageNameQuery();
            Result<string> result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags("Library.Settings");

        app.MapGet("library/settings/background/path", async (
            IQueryHandler<GetBackgroundImagePathQuery, string> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetBackgroundImagePathQuery();
            Result<string> result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags("Library.Settings");
    }
}


