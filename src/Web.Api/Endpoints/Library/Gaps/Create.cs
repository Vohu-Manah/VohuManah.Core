using Application.Abstractions.Messaging;
using Application.Library.Gaps.Create;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Gaps;

internal sealed class Create : IEndpoint
{
    public sealed record Request(string Title, string Note, bool State);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("library/gaps", async (
            Request request,
            ICommandHandler<CreateGapCommand, int> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateGapCommand(request.Title, request.Note, request.State);

            Result<int> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags("Library.Gaps");
    }
}

