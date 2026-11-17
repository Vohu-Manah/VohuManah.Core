using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Manuscripts.Delete;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Manuscripts;

[RequireRole(PermissionNames.Manuscripts.Delete)]
internal sealed class Delete : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("library/manuscripts/{id:int}", async (
            int id,
            ICommandHandler<DeleteManuscriptCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteManuscriptCommand(id);

            Result result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.Ok() : CustomResults.Problem(result);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Manuscripts");
    }
}

