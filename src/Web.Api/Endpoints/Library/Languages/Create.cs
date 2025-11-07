using Application.Abstractions.Messaging;
using Application.Library.Languages.Create;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Languages;

internal sealed class Create : IEndpoint
{
    public sealed record Request(string Name, string Abbreviation);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("library/languages", async (
            Request request,
            ICommandHandler<CreateLanguageCommand, int> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateLanguageCommand(request.Name, request.Abbreviation);

            Result<int> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags("Library.Languages");
    }
}

