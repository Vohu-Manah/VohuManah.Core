using Application.Abstractions.Messaging;
using Application.Library.Settings.GetUserSettings;
using SharedKernel;
using System.Security.Claims;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Application.Library.Settings;

namespace Web.Api.Endpoints.Library.Settings;

[RequireRole(EndpointNames.Settings.GetUserSettings)]
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


