using Application.Abstractions.Messaging;
using Application.Library.Settings.AssignRoleToUser;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Settings;

[RequireRole("Library.Settings.AssignRoleToUser")]
internal sealed class AssignRoleToUser : BaseEndpoint
{
    public sealed record Request(int RoleId);

    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("library/settings/users/{userName}/roles", async (
            string userName,
            Request request,
            ICommandHandler<AssignRoleToUserCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new AssignRoleToUserCommand(userName, request.RoleId);

            Result result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.Ok() : CustomResults.Problem(result);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Settings");
    }
}

