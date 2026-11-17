using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Users.Delete;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Library.Settings;

namespace VohuManah.Web.Api.Endpoints.Library.Users;

[RequireRole(EndpointNames.Users.Delete)]
internal sealed class Delete : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("library/users/{id:long}", async (
            long id,
            ICommandHandler<DeleteLibraryUserCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteLibraryUserCommand(id);

            Result result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.Ok() : CustomResults.Problem(result);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Users");
    }
}

