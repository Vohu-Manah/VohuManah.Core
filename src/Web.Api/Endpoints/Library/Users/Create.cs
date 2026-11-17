using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Users.Create;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Http;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Users;

[RequireRole(PermissionNames.Users.Create)]
internal sealed class Create : BaseEndpoint
{
    public sealed record Request(
        string UserName,
        string Password,
        string Name,
        string LastName);

    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("library/users", async (
            Request request,
            ICommandHandler<CreateLibraryUserCommand, string> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateLibraryUserCommand(
                request.UserName,
                request.Password,
                request.Name,
                request.LastName);

            Result<string> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Users");
    }
}
