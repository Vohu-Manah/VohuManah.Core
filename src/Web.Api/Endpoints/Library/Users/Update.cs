using Application.Abstractions.Messaging;
using Application.Library.Users.Update;
using SharedKernel;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Users;

[RequireRole("Library.Users.Update")]
internal sealed class Update : BaseEndpoint
{
    public sealed record Request(
        string UserName,
        string Password,
        string Name,
        string LastName);

    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("library/users", async (
            Request request,
            ICommandHandler<UpdateLibraryUserCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateLibraryUserCommand(
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

