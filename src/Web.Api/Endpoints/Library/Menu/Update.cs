using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Menu.GetAll;
using VohuManah.Application.Library.Menu.Update;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Http;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Menu;

[RequireRole(PermissionNames.Menu.Update)]
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

