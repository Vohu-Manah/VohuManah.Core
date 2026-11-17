using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Settings.GetAll;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Settings;

[RequireRole(PermissionNames.Settings.GetAll)]
internal sealed class GetAll : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/settings", async (
            IQueryHandler<GetAllSettingsQuery, List<SettingsResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllSettingsQuery();

            Result<List<SettingsResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Settings");
    }
}

