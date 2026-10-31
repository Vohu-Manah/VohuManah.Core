using Application.Abstractions.Messaging;
using Application.Library.PublicationTypes.Update;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;
using Application.Library.Settings;

namespace Web.Api.Endpoints.Library.PublicationTypes;

[RequireRole(EndpointNames.PublicationTypes.Update)]
internal sealed class Update : BaseEndpoint
{
    public sealed record Request(string Title);

    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("library/publication-types/{id:int}", async (
            int id,
            Request request,
            ICommandHandler<UpdatePublicationTypeCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdatePublicationTypeCommand(id, request.Title);

            Result result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.Ok() : CustomResults.Problem(result);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.PublicationTypes");
    }
}

