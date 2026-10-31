using Application.Abstractions.Messaging;
using Application.Library.Publishers.Delete;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;
using Application.Library.Settings;

namespace Web.Api.Endpoints.Library.Publishers;

[RequireRole(EndpointNames.Publishers.Delete)]
internal sealed class Delete : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("library/publishers/{id:int}", async (
            int id,
            ICommandHandler<DeletePublisherCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeletePublisherCommand(id);

            Result result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.Ok() : CustomResults.Problem(result);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Publishers");
    }
}

