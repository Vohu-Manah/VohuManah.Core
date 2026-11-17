using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Subjects.Update;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Subjects;

[RequireRole(PermissionNames.Subjects.Update)]
internal sealed class Update : BaseEndpoint
{
    public sealed record Request(string Title);

    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("library/subjects/{id:int}", async (
            int id,
            Request request,
            ICommandHandler<UpdateSubjectCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateSubjectCommand(id, request.Title);

            Result result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.Ok() : CustomResults.Problem(result);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Subjects");
    }
}

