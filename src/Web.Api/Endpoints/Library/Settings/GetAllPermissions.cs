using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Abstractions.Authorization;
using VohuManah.Application.Library.Settings.GetAllPermissions;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;

namespace VohuManah.Web.Api.Endpoints.Library.Settings;

[RequireRole(PermissionNames.Settings.GetAllEndpoints)]
internal sealed class GetAllPermissions : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/settings/endpoints", async (
            IQueryHandler<GetAllPermissionsQuery, Dictionary<string, List<string>>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllPermissionsQuery();

            Result<Dictionary<string, List<string>>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Settings");
    }
}

