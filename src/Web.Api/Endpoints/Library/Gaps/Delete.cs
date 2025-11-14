using Application.Abstractions.Messaging;
using Application.Library.Gaps.Delete;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Gaps;

[RequireRole("Library.Gaps.Delete")]
internal sealed class Delete : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("library/gaps/{id:int}", async (
            int id,
            ICommandHandler<DeleteGapCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteGapCommand(id);

            Result result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.Ok() : CustomResults.Problem(result);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Gaps");
    }
}

