using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Settings.UpdateUserSettings;
using VohuManah.SharedKernel;
using System.Security.Claims;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Http;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Settings;

[RequireRole(PermissionNames.Settings.UpdateUserSettings)]
internal sealed class UpdateUserSettingsEndpoint : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("library/settings/user-settings", async (
            HttpContext httpContext,
            UpdateUserSettingsRequest request,
            ICommandHandler<UpdateUserSettingsCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
            {
                return CustomResults.Problem(
                    title: "Ø®Ø·Ø§ Ø¯Ø± Ø§Ø­Ø±Ø§Ø² Ù‡ÙˆÛŒØª",
                    detail: "Ø´Ù†Ø§Ø³Ù‡ Ú©Ø§Ø±Ø¨Ø± ÛŒØ§ÙØª Ù†Ø´Ø¯");
            }

            var command = new UpdateUserSettingsCommand(
                userId,
                request.ProfileImageAttachmentId,
                request.ThemePrimaryColor,
                request.ThemeSecondaryColor,
                request.ThemeAccentColor,
                request.ThemeMode);

            Result result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Settings");
    }
}

internal sealed record UpdateUserSettingsRequest
{
    public long? ProfileImageAttachmentId { get; init; }
    public string? ThemePrimaryColor { get; init; }
    public string? ThemeSecondaryColor { get; init; }
    public string? ThemeAccentColor { get; init; }
    public string? ThemeMode { get; init; }
}

