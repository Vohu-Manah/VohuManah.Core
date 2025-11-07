using Application.Abstractions.Messaging;
using Application.Library.Subjects.Delete;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Subjects;

internal sealed class Delete : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
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
        .WithTags("Library.Subjects");
    }
}

