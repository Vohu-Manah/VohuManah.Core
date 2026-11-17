using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Settings.GetMainTitle;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Settings;

[RequireRole(PermissionNames.Settings.GetMainTitle)]
internal sealed class GetMainTitle : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/settings/main-title", async (
            IQueryHandler<GetApplicationMainTitleQuery, string> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetApplicationMainTitleQuery();
            Result<string> result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Settings");
    }
}


