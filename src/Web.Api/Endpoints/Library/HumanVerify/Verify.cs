using Application.Abstractions.HumanVerification;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.HumanVerify;

internal sealed class Verify : IEndpoint
{
    public sealed record Request(string ChallengeId, string Signature, int HeldMs, string Mode, List<PointerSample> PointerSamples, int KeyRepeats, string Honeypot);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("library/human-verify/verify", async (
            Request request,
            IHumanVerificationService service,
            CancellationToken cancellationToken) =>
        {
            Result<HumanVerifyToken> result = await service.VerifyAsync(
                request.ChallengeId,
                request.Signature,
                request.HeldMs,
                request.Mode ?? "pointer",
                request.PointerSamples ?? new List<PointerSample>(),
                request.KeyRepeats,
                request.Honeypot ?? string.Empty,
                cancellationToken);

            return result.Match(
                token => Results.Ok(new { VerifiedToken = token.Token, ExpiresInSeconds = token.ExpiresInSeconds }),
                CustomResults.Problem);
        })
        .AllowAnonymous()
        .WithTags("Library.Users");
    }
}
