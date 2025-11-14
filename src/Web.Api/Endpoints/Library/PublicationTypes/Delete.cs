using Application.Abstractions.Messaging;
using Application.Library.PublicationTypes.Delete;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.PublicationTypes;

[RequireRole("Library.PublicationTypes.Delete")]
internal sealed class Delete : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("library/publication-types/{id:int}", async (
            int id,
            ICommandHandler<DeletePublicationTypeCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeletePublicationTypeCommand(id);

            Result result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.Ok() : CustomResults.Problem(result);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.PublicationTypes");
    }
}

