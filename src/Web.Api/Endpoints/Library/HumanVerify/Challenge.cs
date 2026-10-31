using Application.Abstractions.HumanVerification;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.HumanVerify;

internal sealed class Challenge : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("library/human-verify/challenge", async (
            IHumanVerificationService service,
            CancellationToken cancellationToken) =>
        {
            Result<HumanVerifyChallenge> result = await service.CreateChallengeAsync(cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .AllowAnonymous()
        .WithTags("Library.Users");
    }
}
