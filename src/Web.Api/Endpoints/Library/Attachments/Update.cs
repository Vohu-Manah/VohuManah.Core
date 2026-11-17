using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Attachments.Update;
using VohuManah.Application.Library.Settings;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;

namespace VohuManah.Web.Api.Endpoints.Library.Attachments;

[RequireRole(EndpointNames.Attachments.Update)]
internal sealed class Update : BaseEndpoint
{
    public sealed record Request(string? Description, bool? IsMain);

    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("library/attachments/{id:long}", async (
            long id,
            Request request,
            ICommandHandler<UpdateAttachmentCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateAttachmentCommand(
                id,
                request.Description,
                request.IsMain);

            Result result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.Ok() : CustomResults.Problem(result);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Attachments");
    }
}

