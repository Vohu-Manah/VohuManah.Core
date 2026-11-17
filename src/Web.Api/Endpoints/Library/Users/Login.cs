using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Users.Login;
using VohuManah.Web.Api.Security;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;

namespace VohuManah.Web.Api.Endpoints.Library.Users;

internal sealed class Login : IEndpoint
{
    public sealed record Request(string UserName, string Password, string? HumanVerifyToken);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("library/users/login", async (
            Request request,
            HttpContext httpContext,
            ICommandHandler<LoginLibraryUserCommand, LoginLibraryUserResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new LoginLibraryUserCommand(request.UserName, request.Password, request.HumanVerifyToken);

            Result<LoginLibraryUserResponse> result = await handler.Handle(command, cancellationToken);

            if (result.IsSuccess && !string.IsNullOrEmpty(result.Value.AccessToken) && !string.IsNullOrEmpty(result.Value.RefreshToken))
            {
                AuthCookies.SetAuthCookies(httpContext, result.Value.AccessToken, result.Value.RefreshToken);
            }

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .AllowAnonymous()
        .WithTags("Library.Users");
    }
}
