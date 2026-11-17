using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Users.Update;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Http;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Users;

[RequireRole(PermissionNames.Users.Update)]
internal sealed class Update : BaseEndpoint
{
    public sealed record Request(
        string UserName,
        string Password,
        string Name,
        string LastName);

    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        
        
        app.MapPut("library/users/{id:long}", async (
            long id,
            Request request,
            ICommandHandler<UpdateLibraryUserCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateLibraryUserCommand(
                id,
                request.UserName,
                request.Password,
                request.Name,
                request.LastName);

            Result result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.Ok() : CustomResults.Problem(result);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Users");
    }
}

