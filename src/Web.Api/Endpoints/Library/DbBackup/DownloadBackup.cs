using Application.Abstractions.Messaging;
using Application.Library.Backup.Download;
using Application.Library.Settings;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Backup;

[RequireRole(EndpointNames.Backup.Download)]
internal sealed class DownloadBackup : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/backup/download", async (
            string? fileName,
            IQueryHandler<DownloadBackupQuery, BackupDownloadFile> handler,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var query = new DownloadBackupQuery(fileName ?? Application.Library.Backup.Shared.BackupFileNames.Daily);

            Result<BackupDownloadFile> result = await handler.Handle(query, cancellationToken);

            if (result.IsFailure)
            {
                return CustomResults.Problem(result);
            }

            var file = result.Value;

            
            
            httpContext.Response.Headers["X-Accel-Buffering"] = "no";
            httpContext.Response.Headers["Content-Type"] = "application/octet-stream";
            httpContext.Response.Headers["Content-Length"] = file.ContentLength.ToString();
            httpContext.Response.Headers["Content-Disposition"] = $"attachment; filename=\"{file.FileName}\"";

            
            await file.Content.CopyToAsync(httpContext.Response.Body, 64 * 1024, cancellationToken);
            await httpContext.Response.Body.FlushAsync(cancellationToken);

            return Results.Empty;
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Backup");
    }
}
