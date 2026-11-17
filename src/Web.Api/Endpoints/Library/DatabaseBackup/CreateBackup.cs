using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.DatabaseBackup.Create;
using VohuManah.Application.Library.Settings;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;

namespace VohuManah.Web.Api.Endpoints.Library.DatabaseBackup;

[RequireRole(EndpointNames.Backup.Create)]
internal sealed class CreateBackup : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("library/backup", async (
            ICommandHandler<CreateBackupCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new CreateBackupCommand(), cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Backup");
    }
}
