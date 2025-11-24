using Application.Abstractions.Messaging;
using Application.Library.Settings.CreateRole;
using Application.Library.Settings.GetAllRoles;
using SharedKernel;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Settings;

[RequireRole("Library.Settings.CreateRole")]
internal sealed class CreateRole : BaseEndpoint
{
    public sealed record Request(string Name);

    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("library/settings/roles", async (
            Request request,
            ICommandHandler<CreateRoleCommand, RoleResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateRoleCommand(request.Name);

            Result<RoleResponse> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Settings");
    }
}

