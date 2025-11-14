using Application.Abstractions.Messaging;
using Application.Library.Gaps.Update;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Gaps;

[RequireRole("Library.Gaps.Update")]
internal sealed class Update : BaseEndpoint
{
    public sealed record Request(int Id, string Title, string Note, bool State);

    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("library/gaps", async (
            Request request,
            ICommandHandler<UpdateGapCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateGapCommand(request.Id, request.Title, request.Note, request.State);

            Result result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.Ok() : CustomResults.Problem(result);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Gaps");
    }
}

