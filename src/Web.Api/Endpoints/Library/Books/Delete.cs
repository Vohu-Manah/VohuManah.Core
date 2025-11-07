using Application.Abstractions.Messaging;
using Application.Library.Books.Delete;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Books;

internal sealed class Delete : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
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
        .WithTags("Library.Books");
    }
}

