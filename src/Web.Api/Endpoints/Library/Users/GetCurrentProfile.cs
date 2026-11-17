using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Users.GetCurrentProfile;
using VohuManah.SharedKernel;
using System.Security.Claims;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Library.Settings;

namespace VohuManah.Web.Api.Endpoints.Library.Users;

[RequireRole(EndpointNames.Settings.GetUserSettings)]
internal sealed class GetCurrentProfile : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/users/profile", async (
            HttpContext httpContext,
            IQueryHandler<GetCurrentUserProfileQuery, UserProfileResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
            {
                return CustomResults.Problem(
                    title: "Ø®Ø·Ø§ Ø¯Ø± Ø§Ø­Ø±Ø§Ø² Ù‡ÙˆÛŒØª",
                    detail: "Ø´Ù†Ø§Ø³Ù‡ Ú©Ø§Ø±Ø¨Ø± ÛŒØ§ÙØª Ù†Ø´Ø¯");
            }

            var query = new GetCurrentUserProfileQuery(userId);
            Result<UserProfileResponse> result = await handler.Handle(query, cancellationToken);

            return result.IsSuccess ? Results.Ok(result.Value) : CustomResults.Problem(result);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Users");
    }
}

