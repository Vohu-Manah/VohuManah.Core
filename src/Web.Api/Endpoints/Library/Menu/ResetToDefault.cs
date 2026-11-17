using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Menu.Reset;
using VohuManah.Application.Library.Settings;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;

namespace VohuManah.Web.Api.Endpoints.Library.Menu;

[RequireRole(EndpointNames.Menu.ResetToDefault)]
internal sealed class ResetToDefault : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("library/menu/reset-to-default", async (
            ICommandHandler<ResetMenuItemsCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new ResetMenuItemsCommand(), cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Menu");
    }
}