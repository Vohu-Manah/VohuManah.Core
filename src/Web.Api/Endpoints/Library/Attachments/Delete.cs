using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Attachments.Delete;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Library.Settings;

namespace VohuManah.Web.Api.Endpoints.Library.Attachments;

[RequireRole(EndpointNames.Attachments.Delete)]
internal sealed class Delete : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("library/attachments/{id:long}", async (
            long id,
            ICommandHandler<DeleteAttachmentCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteAttachmentCommand(id);

            Result result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.Ok() : CustomResults.Problem(result);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Attachments");
    }
}


