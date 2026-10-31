using Application.Abstractions.Messaging;
using Application.Library.Publishers.Update;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;
using Application.Library.Settings;

namespace Web.Api.Endpoints.Library.Publishers;

[RequireRole(EndpointNames.Publishers.Update)]
internal sealed class Update : BaseEndpoint
{
    public sealed record Request(
        string Name,
        string ManagerName,
        int PlaceId,
        string Address,
        string Telephone,
        string Website,
        string Email);

    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("library/publishers/{id:int}", async (
            int id,
            Request request,
            ICommandHandler<UpdatePublisherCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdatePublisherCommand(
                id,
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
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Publishers");
    }
}

