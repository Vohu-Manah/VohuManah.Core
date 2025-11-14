using Application.Abstractions.Messaging;
using Application.Library.Subjects.Update;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Subjects;

[RequireRole("Library.Subjects.Update")]
internal sealed class Update : BaseEndpoint
{
    public sealed record Request(int Id, string Title);

    public override void MapEndpoint(IEndpointRouteBuilder app)
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
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Subjects");
    }
}

