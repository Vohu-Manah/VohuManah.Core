using Application.Abstractions.Messaging;
using Application.Library.Backup.Create;
using Application.Library.Settings;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Backup;

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
