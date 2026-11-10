using Application.Abstractions.Messaging;
using Application.Library.Backup.Restore;
using Application.Library.Settings;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Backup;

[RequireRole(EndpointNames.Backup.Restore)]
internal sealed class RestoreBackup : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("library/backup/restore", async (
            HttpRequest request,
            ICommandHandler<RestoreBackupCommand> handler,
            CancellationToken cancellationToken) =>
        {
            if (!request.HasFormContentType)
            {
                return Results.BadRequest("درخواست باید از نوع multipart/form-data باشد");
            }

            
            var restoreBodySizeFeature = request.HttpContext.Features.Get<Microsoft.AspNetCore.Http.Features.IHttpMaxRequestBodySizeFeature>();
            if (restoreBodySizeFeature is not null)
            {
                restoreBodySizeFeature.MaxRequestBodySize = 100L * 1024 * 1024 * 1024;
            }

            var formOptions = new Microsoft.AspNetCore.Http.Features.FormOptions
            {
                MultipartBodyLengthLimit = 100L * 1024 * 1024 * 1024,
            };
            IFormCollection form = await request.ReadFormAsync(formOptions, cancellationToken);

            if (form.Files.Count == 0)
            {
                return Results.BadRequest("فایل بکاپ ارسال نشده است");
            }

            IFormFile file = form.Files[0];

            
            if (!file.FileName.EndsWith(".bak", StringComparison.OrdinalIgnoreCase))
            {
                return Results.BadRequest("فقط فایل های با پسوند .bak برای بازگردانی مجاز هستند");
            }

            await using Stream stream = file.OpenReadStream();

            var command = new RestoreBackupCommand(
                BackupRestoreSource.Uploaded,
                stream,
                file.FileName);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Backup")
        .DisableAntiforgery();
    }
}
