using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.DatabaseBackup.GetInfo;
using VohuManah.Application.Library.DatabaseBackup.Shared;
using VohuManah.Application.Library.Settings;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;

namespace VohuManah.Web.Api.Endpoints.Library.DatabaseBackup;

[RequireRole(EndpointNames.Backup.GetInfo)]
internal sealed class GetBackupInfo : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/backup", async (
            IQueryHandler<GetBackupInfoQuery, BackupInfoResponse> handler,
            CancellationToken cancellationToken) =>
        {
            Result<BackupInfoResponse> result = await handler.Handle(new GetBackupInfoQuery(), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Backup");
    }
}
