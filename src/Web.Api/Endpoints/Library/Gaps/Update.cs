using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Gaps.Update;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Gaps;

[RequireRole(PermissionNames.Gaps.Update)]
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

