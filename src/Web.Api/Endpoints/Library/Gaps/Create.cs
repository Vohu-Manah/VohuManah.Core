using Application.Abstractions.Messaging;
using Application.Library.Gaps.Create;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Gaps;

[RequireRole("Library.Gaps.Create")]
internal sealed class Create : BaseEndpoint
{
    public sealed record Request(string Title, string Note, bool State);

    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("library/gaps", async (
            Request request,
            ICommandHandler<CreateGapCommand, int> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateGapCommand(request.Title, request.Note, request.State);

            Result<int> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Gaps");
    }
}

