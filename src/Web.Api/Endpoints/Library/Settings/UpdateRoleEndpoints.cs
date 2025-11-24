using System;
using System.Collections.Generic;
using Application.Abstractions.Messaging;
using Application.Library.Settings.UpdateRolePermissions;
using SharedKernel;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Settings;

[RequireRole("Library.Settings.UpdateRoleEndpoints")]
internal sealed class UpdateRoleEndpoints : BaseEndpoint
{
    public sealed record Request(List<string>? EndpointNames);

    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("library/settings/roles/{roleId:int}/endpoints", async (
            int roleId,
            Request request,
            ICommandHandler<UpdateRolePermissionsCommand> handler,
            CancellationToken cancellationToken) =>
        {
            IReadOnlyCollection<string> endpointNames = request.EndpointNames ?? new List<string>();

            var command = new UpdateRolePermissionsCommand(roleId, endpointNames);

            Result result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.Ok() : CustomResults.Problem(result);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Settings");
    }
}

