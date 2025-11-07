using Application.Abstractions.Messaging;
using Application.Library.Subjects.Update;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Subjects;

internal sealed class Update : IEndpoint
{
    public sealed record Request(int Id, string Title);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("library/subjects", async (
            Request request,
            ICommandHandler<UpdateSubjectCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateSubjectCommand(request.Id, request.Title);

            Result result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.Ok() : CustomResults.Problem(result);
        })
        .WithTags("Library.Subjects");
    }
}

