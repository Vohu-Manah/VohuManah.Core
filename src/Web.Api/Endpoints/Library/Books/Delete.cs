using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Books.Delete;
using VohuManah.Application.Abstractions.Authorization;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Infrastructure;

namespace VohuManah.Web.Api.Endpoints.Library.Books;

[RequireRole(PermissionNames.Books.Delete)]
internal sealed class Delete : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("library/books/{id:long}", async (
            long id,
            ICommandHandler<DeleteBookCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteBookCommand(id);

            Result result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.Ok() : CustomResults.Problem(result);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Books");
    }
}

