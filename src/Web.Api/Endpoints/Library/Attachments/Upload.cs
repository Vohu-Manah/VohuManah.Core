using Application.Abstractions.Messaging;
using Application.Library.Attachments.Upload;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Application.Library.Settings;

namespace Web.Api.Endpoints.Library.Attachments;

[RequireRole(EndpointNames.Attachments.Upload)]
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
                return Results.BadRequest("Ù†ÙˆØ¹ Ø¨Ø§ÛŒØ¯ Ø§Ø² multipart/form-data");
            }

            IFormCollection form = await request.ReadFormAsync(cancellationToken);

            if (!form.TryGetValue("entityType", out var entityType) ||
                !form.TryGetValue("entityId", out var entityId) ||
                form.Files.Count == 0)
            {
                return Results.BadRequest("ÙˆØ±ÙˆØ¯ÛŒ Ù†Ø§Ù‚Øµ Ø§Ø³Øª");
            }

            IFormFile file = form.Files[0];
            string? description = form.TryGetValue("description", out var desc) ? desc.ToString() : null;
            bool isMain = form.TryGetValue("isMain", out var isMainValue) && bool.TryParse(isMainValue.ToString(), out bool parsedIsMain) && parsedIsMain;
            string? createdBy = request.HttpContext.User.Identity?.Name;

            if (string.IsNullOrEmpty(createdBy))
            {
                return Results.Unauthorized();
            }

            if (!long.TryParse(entityId, out long entityIdLong))
            {
                return Results.BadRequest("Ø´Ù†Ø§Ø³Ù‡ Ù†Ø§Ù…Ø¹ØªØ¨Ø±");
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
                createdBy,
                isMain);

            Result<long> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Attachments")
        .DisableAntiforgery();
    }
}


