using Application.Abstractions.Messaging;
using Application.Library.PublicationTypes.Delete;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.PublicationTypes;

internal sealed class Delete : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("library/publication-types/{id:int}", async (
            int id,
            ICommandHandler<DeletePublicationTypeCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeletePublicationTypeCommand(id);

            Result result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.Ok() : CustomResults.Problem(result);
        })
        .WithTags("Library.PublicationTypes");
    }
}

