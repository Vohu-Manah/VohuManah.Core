using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Settings.GetBackground;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Http;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Settings;

[RequireRole(PermissionNames.Settings.GetBackground)]
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


