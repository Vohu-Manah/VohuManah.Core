using VohuManah.Application.Abstractions.DatabaseBackup;
using VohuManah.Application.Abstractions.Authorization;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;

namespace VohuManah.Web.Api.Endpoints.Library.DatabaseBackup;





[RequireRole(PermissionNames.Backup.Cancel)]
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
