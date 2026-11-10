using Application.Abstractions.Messaging;
using Application.Library.Backup.Restore;
using Application.Library.Settings;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Backup;

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
