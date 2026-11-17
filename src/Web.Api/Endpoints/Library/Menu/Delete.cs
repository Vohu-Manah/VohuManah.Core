using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Menu.Delete;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Http;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Menu;

[RequireRole(PermissionNames.Menu.Delete)]
internal sealed class Delete : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("library/menu/{id:int}", async (
            int id,
            ICommandHandler<DeleteMenuItemCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteMenuItemCommand(id);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Menu");
    }
}

