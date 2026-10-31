using Application.Abstractions.Messaging;
using Application.Library.Attachments.Update;
using Application.Library.Settings;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Attachments;

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

