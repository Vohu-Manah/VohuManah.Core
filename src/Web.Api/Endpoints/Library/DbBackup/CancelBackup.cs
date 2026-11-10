using Application.Abstractions.Backup;
using Application.Library.Settings;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Backup;





[RequireRole(EndpointNames.Backup.Cancel)]
internal sealed class CancelBackup : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("library/backup/cancel", async (
            IBackupService backupService,
            CancellationToken cancellationToken) =>
        {
            bool cancelled = await backupService.CancelAsync(cancellationToken);

            return Results.Ok(new { cancelled });
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Backup");
    }
}
