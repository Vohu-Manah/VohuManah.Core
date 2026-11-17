using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Settings.GetUserSettings;
using VohuManah.SharedKernel;
using System.Security.Claims;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Settings;

[RequireRole(PermissionNames.Settings.GetUserSettings)]
internal sealed class GetUserSettingsEndpoint : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/settings/user-settings", async (
            HttpContext httpContext,
            IQueryHandler<GetUserSettingsQuery, UserSettingsResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
            {
                return CustomResults.Problem(
                    title: "Ø®Ø·Ø§ Ø¯Ø± Ø§Ø­Ø±Ø§Ø² Ù‡ÙˆÛŒØª",
                    detail: "Ø´Ù†Ø§Ø³Ù‡ Ú©Ø§Ø±Ø¨Ø± ÛŒØ§ÙØª Ù†Ø´Ø¯");
            }

            var query = new GetUserSettingsQuery(userId);
            Result<UserSettingsResponse> result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Settings");
    }
}


