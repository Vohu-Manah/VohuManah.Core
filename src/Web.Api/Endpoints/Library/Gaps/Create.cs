using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Gaps.Create;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Library.Settings;

namespace VohuManah.Web.Api.Endpoints.Library.Gaps;

[RequireRole(EndpointNames.Gaps.Create)]
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

