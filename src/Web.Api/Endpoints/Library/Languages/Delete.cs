using Application.Abstractions.Messaging;
using Application.Library.Languages.Delete;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Languages;

internal sealed class Delete : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("library/languages/{id:int}", async (
            int id,
            ICommandHandler<DeleteLanguageCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteLanguageCommand(id);

            Result result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.Ok() : CustomResults.Problem(result);
        })
        .WithTags("Library.Languages");
    }
}

