using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Gaps.Delete;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Library.Settings;

namespace VohuManah.Web.Api.Endpoints.Library.Gaps;

[RequireRole(EndpointNames.Gaps.Delete)]
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

