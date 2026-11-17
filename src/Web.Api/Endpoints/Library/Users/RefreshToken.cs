using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Users.Tokens;
using VohuManah.Web.Api.Security;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Http;

namespace VohuManah.Web.Api.Endpoints.Library.Users;

internal sealed class RefreshToken : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("library/users/refresh-token", async (
            HttpContext httpContext,
            ICommandHandler<RefreshAccessTokenCommand, RefreshAccessTokenResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var refreshToken = AuthCookies.GetRefreshToken(httpContext);
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Results.Unauthorized();
            }

            var command = new RefreshAccessTokenCommand(refreshToken);
            Result<RefreshAccessTokenResponse> result = await handler.Handle(command, cancellationToken);

            if (result.IsSuccess && !string.IsNullOrEmpty(result.Value.AccessToken) && !string.IsNullOrEmpty(result.Value.RefreshToken))
            {
                AuthCookies.SetAuthCookies(httpContext, result.Value.AccessToken, result.Value.RefreshToken);
            }

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .AllowAnonymous()
        .WithTags("Library.Users")
        .RequireCors("CorsPolicy");
    }
}


