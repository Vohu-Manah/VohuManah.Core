using Application.Abstractions.Messaging;
using Application.Library.Menu.GetAll;
using Application.Library.Menu.Update;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Application.Library.Settings;

namespace Web.Api.Endpoints.Library.Menu;

[RequireRole(EndpointNames.Menu.Update)]
internal sealed class Update : BaseEndpoint
{
    public sealed record Request(
        int Id,
        string Label,
        string? Path,
        string? Icon,
        int? ParentId,
        int DisplayOrder,
        bool IsActive);

    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("library/menu/{id:int}", async (
            int id,
            Request request,
            ICommandHandler<UpdateMenuItemCommand, MenuItemResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateMenuItemCommand(
                id,
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

