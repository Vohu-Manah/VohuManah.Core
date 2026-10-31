using Application.Abstractions.Messaging;
using Application.Library.Settings;
using Application.Library.Users.UpdateProfile;
using SharedKernel;
using System.Security.Claims;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Users;

[RequireRole(EndpointNames.Settings.UpdateUserSettings)]
internal sealed class UpdateProfile : BaseEndpoint
{
    public sealed record Request(string Name, string LastName, string? Email, string? PhoneNumber);

    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("library/users/profile", async (
            HttpContext httpContext,
            Request request,
            ICommandHandler<UpdateUserProfileCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
            {
                return CustomResults.Problem(
                    title: "خطا در احراز هویت",
                    detail: "شناسه کاربر یافت نشد");
            }

            var command = new UpdateUserProfileCommand(
                userId,
                request.Name,
                request.LastName,
                request.Email,
                request.PhoneNumber);

            Result result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.NoContent() : CustomResults.Problem(result);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Users");
    }
}

