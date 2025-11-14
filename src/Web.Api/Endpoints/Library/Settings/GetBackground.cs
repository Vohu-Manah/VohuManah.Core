using Application.Abstractions.Messaging;
using Application.Library.Settings.GetBackground;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Settings;

[RequireRole("Library.Settings.GetBackground")]
internal sealed class GetBackground : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/settings/background/folder", async (
            IQueryHandler<GetCurrentBackgroundImageFolderQuery, string> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetCurrentBackgroundImageFolderQuery();
            Result<string> result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Settings");

        app.MapGet("library/settings/background/name", async (
            IQueryHandler<GetCurrentBackgroundImageNameQuery, string> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetCurrentBackgroundImageNameQuery();
            Result<string> result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Settings");

        app.MapGet("library/settings/background/path", async (
            IQueryHandler<GetBackgroundImagePathQuery, string> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetBackgroundImagePathQuery();
            Result<string> result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Settings");
    }
}


