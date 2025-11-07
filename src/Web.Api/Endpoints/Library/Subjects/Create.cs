using Application.Abstractions.Messaging;
using Application.Library.Subjects.Create;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Subjects;

internal sealed class Create : IEndpoint
{
    public sealed record Request(string Title);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("library/subjects", async (
            Request request,
            ICommandHandler<CreateSubjectCommand, int> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateSubjectCommand(request.Title);

            Result<int> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags("Library.Subjects");
    }
}

