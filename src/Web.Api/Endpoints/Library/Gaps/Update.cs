using Application.Abstractions.Messaging;
using Application.Library.Gaps.Update;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;
using Application.Library.Settings;

namespace Web.Api.Endpoints.Library.Gaps;

[RequireRole(EndpointNames.Gaps.Update)]
internal sealed class Update : BaseEndpoint
{
    public sealed record Request(string Title, string Note, bool State);

    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("library/gaps/{id:int}", async (
            int id,
            Request request,
            ICommandHandler<UpdateGapCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateGapCommand(id, request.Title, request.Note, request.State);

            Result result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.Ok() : CustomResults.Problem(result);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Gaps");
    }
}

