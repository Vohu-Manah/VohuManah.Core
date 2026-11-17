using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Users.Tokens;
using VohuManah.Web.Api.Security;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Http;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Users;

[RequireRole(PermissionNames.Users.Revoke)]
internal sealed class Revoke : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("library/users/revoke-token", async (
            HttpContext httpContext,
            ICommandHandler<RevokeRefreshTokenCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var refreshToken = AuthCookies.GetRefreshToken(httpContext);
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Results.Ok();
            }

            var command = new RevokeRefreshTokenCommand(refreshToken);
            var result = await handler.Handle(command, cancellationToken);

            AuthCookies.ClearAuthCookies(httpContext);

            return result.IsSuccess ? Results.Ok() : CustomResults.Problem(result);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Users");
    }
}


