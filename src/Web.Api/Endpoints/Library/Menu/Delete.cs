using Application.Abstractions.Messaging;
using Application.Library.Menu.Delete;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Application.Library.Settings;

namespace Web.Api.Endpoints.Library.Menu;

[RequireRole(EndpointNames.Menu.Delete)]
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

