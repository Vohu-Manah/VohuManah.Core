using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.PublicationTypes.Delete;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.PublicationTypes;

[RequireRole(PermissionNames.PublicationTypes.Delete)]
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

