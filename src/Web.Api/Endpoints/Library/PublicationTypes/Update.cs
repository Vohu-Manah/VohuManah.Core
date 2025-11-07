using Application.Abstractions.Messaging;
using Application.Library.PublicationTypes.Update;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.PublicationTypes;

internal sealed class Update : IEndpoint
{
    public sealed record Request(int Id, string Title);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("library/publication-types", async (
            Request request,
            ICommandHandler<UpdatePublicationTypeCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdatePublicationTypeCommand(request.Id, request.Title);

            Result result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.Ok() : CustomResults.Problem(result);
        })
        .WithTags("Library.PublicationTypes");
    }
}

