using Application.Abstractions.Messaging;
using Application.Library.Publications.Delete;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Publications;

internal sealed class Delete : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("library/publications/{id:int}", async (
            int id,
            ICommandHandler<DeletePublicationCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeletePublicationCommand(id);

            Result result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.Ok() : CustomResults.Problem(result);
        })
        .WithTags("Library.Publications");
    }
}

