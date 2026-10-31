using Application.Abstractions.Messaging;
using Application.Library.Menu.Reorder;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Application.Library.Settings;

namespace Web.Api.Endpoints.Library.Menu;

[RequireRole(EndpointNames.Menu.Reorder)]
internal sealed class Reorder : BaseEndpoint
{
    public sealed record Request(List<MenuItemOrderRequest> Items);

    public sealed record MenuItemOrderRequest(
        int Id,
        int DisplayOrder,
        int? ParentId);

    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("library/menu/reorder", async (
            Request request,
            ICommandHandler<ReorderMenuItemsCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new ReorderMenuItemsCommand(
                request.Items.Select(i => new MenuItemOrder(i.Id, i.DisplayOrder, i.ParentId)).ToList());

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Menu");
    }
}

