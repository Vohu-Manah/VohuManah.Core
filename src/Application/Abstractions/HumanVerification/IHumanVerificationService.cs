using SharedKernel;

namespace Application.Abstractions.HumanVerification;

public sealed record HumanVerifyChallenge(
    string ChallengeId,
    long IssuedAtUnixSeconds,
    string Signature,
    int ExpiresInSeconds,
    int MinHoldMs);

public sealed record HumanVerifyToken(string Token, int ExpiresInSeconds);

public interface IHumanVerificationService
{
    Task<Result<HumanVerifyChallenge>> CreateChallengeAsync(CancellationToken cancellationToken);

    Task<Result<HumanVerifyToken>> VerifyAsync(
        string challengeId,
        string signature,
        int heldMs,
        string mode,
        IReadOnlyList<PointerSample> pointerSamples,
        int keyRepeats,
        string honeypot,
        CancellationToken cancellationToken);

    Task<bool> ConfirmTokenValidAsync(string verifiedToken, CancellationToken cancellationToken);
}
