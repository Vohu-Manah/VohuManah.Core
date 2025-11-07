using Application.Abstractions.Messaging;
using Application.Library.Manuscripts.Delete;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Manuscripts;

internal sealed class Delete : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("library/manuscripts/{id:int}", async (
            int id,
            ICommandHandler<DeleteManuscriptCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteManuscriptCommand(id);

            Result result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.Ok() : CustomResults.Problem(result);
        })
        .WithTags("Library.Manuscripts");
    }
}

