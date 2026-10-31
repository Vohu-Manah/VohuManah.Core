using Application.Abstractions.Messaging;
using Application.Library.Settings;
using Application.Library.Users.ChangePassword;
using SharedKernel;
using System.Security.Claims;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Users;

[RequireRole(EndpointNames.Users.ChangePassword)]
internal sealed class ChangePassword : BaseEndpoint
{
    public sealed record Request(string CurrentPassword, string NewPassword);

    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("library/users/change-password", async (
            HttpContext httpContext,
            Request request,
            ICommandHandler<ChangePasswordCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
            {
                return CustomResults.Problem(
                    title: "خطا در احراز هویت",
                    detail: "شناسه کاربر یافت نشد");
            }

            var command = new ChangePasswordCommand(
                userId,
                request.CurrentPassword,
                request.NewPassword);

            Result result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.NoContent() : CustomResults.Problem(result);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Users");
    }
}

