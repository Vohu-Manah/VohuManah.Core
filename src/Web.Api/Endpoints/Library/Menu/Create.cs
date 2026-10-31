using Application.Abstractions.Messaging;
using Application.Library.Menu.Create;
using Application.Library.Menu.GetAll;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Application.Library.Settings;

namespace Web.Api.Endpoints.Library.Menu;

[RequireRole(EndpointNames.Menu.Create)]
internal sealed class Create : BaseEndpoint
{
    public sealed record Request(
        string Label,
        string? Path,
        string? Icon,
        int? ParentId,
        int DisplayOrder,
        bool IsActive = true);

    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("library/menu", async (
            Request request,
            ICommandHandler<CreateMenuItemCommand, MenuItemResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateMenuItemCommand(
                request.Label,
                request.Path,
                request.Icon,
                request.ParentId,
                request.DisplayOrder,
                request.IsActive);

            Result<MenuItemResponse> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Menu");
    }
}

