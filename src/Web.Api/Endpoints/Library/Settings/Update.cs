using Application.Abstractions.Messaging;
using Application.Library.Settings.Update;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Settings;

internal sealed class Update : IEndpoint
{
    public sealed record Request(string Name, string Value);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("library/settings", async (
            Request request,
            ICommandHandler<UpdateSettingsCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateSettingsCommand(request.Name, request.Value);

            Result result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.Ok() : CustomResults.Problem(result);
        })
        .WithTags("Library.Settings");
    }
}

