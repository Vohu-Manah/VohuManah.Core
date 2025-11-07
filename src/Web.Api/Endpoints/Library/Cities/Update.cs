using Application.Abstractions.Messaging;
using Application.Library.Cities.Update;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Cities;

internal sealed class Update : IEndpoint
{
    public sealed record Request(int Id, string Name);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("library/cities", async (
            Request request,
            ICommandHandler<UpdateCityCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateCityCommand(request.Id, request.Name);

            Result result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.Ok() : CustomResults.Problem(result);
        })
        .WithTags("Library.Cities");
    }
}

