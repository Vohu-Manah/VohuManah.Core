using Application.Abstractions.Messaging;
using Application.Library.Attachments.Upload;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Attachments;

[RequireRole("Library.Attachments.Upload")]
internal sealed class Upload : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("library/attachments", async (
            HttpRequest request,
            ICommandHandler<UploadAttachmentCommand, long> handler,
            CancellationToken cancellationToken) =>
        {
            if (!request.HasFormContentType)
            {
                return Results.BadRequest("Request must be multipart/form-data");
            }

            IFormCollection form = await request.ReadFormAsync(cancellationToken);
            
            if (!form.TryGetValue("entityType", out var entityType) ||
                !form.TryGetValue("entityId", out var entityId) ||
                form.Files.Count == 0)
            {
                return Results.BadRequest("Missing required fields: entityType, entityId, or file");
            }

            IFormFile file = form.Files[0];
            string? description = form.TryGetValue("description", out var desc) ? desc.ToString() : null;
            string? createdBy = request.HttpContext.User.Identity?.Name;

            if (string.IsNullOrEmpty(createdBy))
            {
                return Results.Unauthorized();
            }

            if (!long.TryParse(entityId, out long entityIdLong))
            {
                return Results.BadRequest("Invalid entityId");
            }

            await using Stream fileStream = file.OpenReadStream();
            
            var command = new UploadAttachmentCommand(
                entityType.ToString()!,
                entityIdLong,
                file.FileName,
                file.ContentType,
                file.Length,
                fileStream,
                description,
                createdBy);

            Result<long> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Attachments")
        .DisableAntiforgery(); // File uploads may need this
    }
}


