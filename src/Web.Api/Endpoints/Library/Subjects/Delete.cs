using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Subjects.Delete;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Subjects;

[RequireRole(PermissionNames.Subjects.Delete)]
internal sealed class Delete : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("library/subjects/{id:int}", async (
            int id,
            ICommandHandler<DeleteSubjectCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteSubjectCommand(id);

            Result result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.Ok() : CustomResults.Problem(result);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Subjects");
    }
}

