using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Settings.Update;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Settings;

[RequireRole(PermissionNames.Settings.Update)]
internal sealed class Update : BaseEndpoint
{
    public sealed record Request(string Name, string Value);

    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("library/settings", async (
            Request request,
            ICommandHandler<UpdateSettingsCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateSettingsCommand(request.Name, request.Value);

            Result result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.Ok() : CustomResults.Problem(result);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Settings");
    }
}

