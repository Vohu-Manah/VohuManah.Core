using Application.Abstractions.Messaging;
using Application.Library.Settings.GetMainTitle;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Settings;

[RequireRole("Library.Settings.GetMainTitle")]
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


