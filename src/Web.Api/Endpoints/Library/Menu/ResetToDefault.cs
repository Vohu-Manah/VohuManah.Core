using Application.Abstractions.Messaging;
using Application.Library.Menu.Reset;
using Application.Library.Settings;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Menu;

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