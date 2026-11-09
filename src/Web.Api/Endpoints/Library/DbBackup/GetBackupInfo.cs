using Application.Abstractions.Messaging;
using Application.Library.Backup.GetInfo;
using Application.Library.Backup.Shared;
using Application.Library.Settings;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Backup;

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
