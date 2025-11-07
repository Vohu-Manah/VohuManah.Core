using Application.Abstractions.Messaging;
using Application.Library.Publishers.Update;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Publishers;

internal sealed class Update : IEndpoint
{
    public sealed record Request(
        int Id,
        string Name,
        string ManagerName,
        int PlaceId,
        string Address,
        string Telephone,
        string Website,
        string Email);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("library/publishers", async (
            Request request,
            ICommandHandler<UpdatePublisherCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdatePublisherCommand(
                request.Id,
                request.Name,
                request.ManagerName,
                request.PlaceId,
                request.Address,
                request.Telephone,
                request.Website,
                request.Email);

            Result result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.Ok() : CustomResults.Problem(result);
        })
        .WithTags("Library.Publishers");
    }
}

