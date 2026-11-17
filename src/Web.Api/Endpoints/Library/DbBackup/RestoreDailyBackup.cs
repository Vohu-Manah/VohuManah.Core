using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.DbBackup.Restore;
using VohuManah.Application.Library.Settings;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;

namespace VohuManah.Web.Api.Endpoints.Library.DbBackup;

[RequireRole(EndpointNames.Backup.Restore)]
internal sealed class RestoreDailyBackup : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("library/backup/restore-daily", async (
            ICommandHandler<RestoreBackupCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RestoreBackupCommand(BackupRestoreSource.DailyFile, null, null);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Backup");
    }
}
