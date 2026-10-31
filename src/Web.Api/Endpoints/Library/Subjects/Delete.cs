using Application.Abstractions.Messaging;
using Application.Library.Subjects.Delete;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;
using Application.Library.Settings;

namespace Web.Api.Endpoints.Library.Subjects;

[RequireRole(EndpointNames.Subjects.Delete)]
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

