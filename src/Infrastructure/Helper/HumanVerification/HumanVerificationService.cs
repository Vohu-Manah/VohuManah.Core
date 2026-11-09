using System.Threading;
using System;
using Application.Abstractions.Caching;
using Application.Abstractions.HumanVerification;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SharedKernel;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;

namespace VohuManah.Infrastructure.Helper.HumanVerification;

internal sealed record ChallengeState(long IssuedAtUnixSeconds);

internal sealed record ChallengeToken(long IssuedAtUnixSeconds);

internal sealed class HumanVerificationService : IHumanVerificationService
{
    private const int ChallengeTtlSeconds = 120;
    private const int TokenTtlSeconds = 120;
    private const int MinHoldMs = 1500;
    private const int MaxHoldMs = 60000;
    private const int RateLimitWindowSeconds = 60;
    private const int RateLimitMax = 12;

    
    private const double MinPointerSpanMs = 60d;
    private const int MinKeyRepeats = 4;

    private readonly ICacheManager _cache;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly byte[] _secret;
    private readonly ConcurrentDictionary<string, (long WindowStart, int Count)> _rateLimits = new();

    public HumanVerificationService(
        ICacheManager cache,
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration)
    {
        _cache = cache;
        _httpContextAccessor = httpContextAccessor;

        string secret =
            configuration["HumanVerification:Secret"] ??
            configuration["Jwt:Secret"] ??
            throw new InvalidOperationException("HumanVerification secret is not configured.");

        _secret = Encoding.UTF8.GetBytes(secret);
    }

    public Task<Result<HumanVerifyChallenge>> CreateChallengeAsync(CancellationToken cancellationToken)
    {
        if (!TryConsumeRateLimit())
            return Task.FromResult(Result.Failure<HumanVerifyChallenge>(HumanVerifyErrors.RateLimited));

        string id = Guid.NewGuid().ToString("N");
        long issuedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        string signature = Sign(id, issuedAt);

        _cache.Store($"hv:challenge:{id}", new ChallengeState(issuedAt), ChallengeTtlSeconds);

        HumanVerifyChallenge challenge = new(id, issuedAt, signature, ChallengeTtlSeconds, MinHoldMs);
        return Task.FromResult(Result.Success(challenge));
    }

    public Task<Result<HumanVerifyToken>> VerifyAsync(
        string challengeId,
        string signature,
        int heldMs,
        string mode,
        IReadOnlyList<PointerSample> pointerSamples,
        int keyRepeats,
        string honeypot,
        CancellationToken cancellationToken)
    {
        if (!TryConsumeRateLimit())
            return Task.FromResult(Result.Failure<HumanVerifyToken>(HumanVerifyErrors.RateLimited));

        string key = $"hv:challenge:{challengeId}";
        ChallengeState? state = _cache.Retrieve<ChallengeState>(key);
        if (state is null)
            return Task.FromResult(Result.Failure<HumanVerifyToken>(HumanVerifyErrors.InvalidOrExpired));

        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        long elapsedMs = (now - state.IssuedAtUnixSeconds) * 1000L;

        
        if (elapsedMs < MinHoldMs || elapsedMs > ChallengeTtlSeconds * 1000L)
            return Task.FromResult(Result.Failure<HumanVerifyToken>(HumanVerifyErrors.TooFast));

        if (heldMs < MinHoldMs || heldMs > MaxHoldMs)
            return Task.FromResult(Result.Failure<HumanVerifyToken>(HumanVerifyErrors.TooFast));

        if (!ConstantTimeEquals(signature, Sign(challengeId, state.IssuedAtUnixSeconds)))
            return Task.FromResult(Result.Failure<HumanVerifyToken>(HumanVerifyErrors.InvalidOrExpired));

        
        if (!string.IsNullOrWhiteSpace(honeypot))
            return Task.FromResult(Result.Failure<HumanVerifyToken>(HumanVerifyErrors.AutomationDetected));

        
        bool humanLike = mode == "key"
            ? keyRepeats >= MinKeyRepeats
            : EvaluatePointerSamples(pointerSamples ?? new List<PointerSample>());

        if (!humanLike)
            return Task.FromResult(Result.Failure<HumanVerifyToken>(HumanVerifyErrors.AutomationDetected));

        
        _cache.Remove(key);

        string token = Guid.NewGuid().ToString("N");
        _cache.Store($"hv:token:{token}", new ChallengeToken(now), TokenTtlSeconds);

        return Task.FromResult(Result.Success(new HumanVerifyToken(token, TokenTtlSeconds)));
    }

    public Task<bool> ConfirmTokenValidAsync(string verifiedToken, CancellationToken cancellationToken)
    {
        string key = $"hv:token:{verifiedToken}";
        ChallengeToken? cached = _cache.Retrieve<ChallengeToken>(key);
        if (cached is null)
            return Task.FromResult(false);

        
        _cache.Remove(key);
        return Task.FromResult(true);
    }

    private static bool EvaluatePointerSamples(IReadOnlyList<PointerSample> samples)
    {
        
        
        if (samples.Count <= 2)
            return true;

        double minT = samples.Min(s => s.T);
        double maxT = samples.Max(s => s.T);
        if ((maxT - minT) < MinPointerSpanMs)
            return true; 

        double pathLength = 0;
        for (int i = 1; i < samples.Count; i++)
            pathLength += Math.Sqrt(Math.Pow(samples[i].X - samples[i - 1].X, 2) + Math.Pow(samples[i].Y - samples[i - 1].Y, 2));

        var first = samples[0];
        var last = samples[^1];
        double dx = last.X - first.X;
        double dy = last.Y - first.Y;
        double denom = Math.Sqrt(dx * dx + dy * dy);
        double maxDev = 0;
        foreach (var s in samples)
        {
            double dev = denom > 0
                ? Math.Abs(dy * (s.X - first.X) - dx * (s.Y - first.Y)) / denom
                : Math.Sqrt(Math.Pow(s.X - first.X, 2) + Math.Pow(s.Y - first.Y, 2));
            if (dev > maxDev) maxDev = dev;
        }

        var speeds = new List<double>();
        for (int i = 1; i < samples.Count; i++)
        {
            double dist = Math.Sqrt(Math.Pow(samples[i].X - samples[i - 1].X, 2) + Math.Pow(samples[i].Y - samples[i - 1].Y, 2));
            double dt = Math.Max(1, samples[i].T - samples[i - 1].T);
            speeds.Add(dist / dt);
        }
        double mean = speeds.Count > 0 ? speeds.Average() : 0;
        double variance = speeds.Count > 0 ? speeds.Average(v => (v - mean) * (v - mean)) : 0;
        double cv = mean > 0 ? Math.Sqrt(variance) / mean : 0;

        
        
        bool isBot = samples.Count >= 15 && pathLength >= 100d && maxDev < 0.5d && cv < 0.05d;
        return !isBot;
    }

    private bool TryConsumeRateLimit()
    {
        string? ip = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        if (string.IsNullOrEmpty(ip))
            return true;

        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        (long WindowStart, int Count) entry = _rateLimits.GetOrAdd(ip, _ => (now, 0));

        if (now - entry.WindowStart >= RateLimitWindowSeconds)
            entry = (now, 0);

        if (entry.Count >= RateLimitMax)
            return false;

        _rateLimits[ip] = (entry.WindowStart, entry.Count + 1);
        return true;
    }

    private string Sign(string id, long issuedAt)
    {
        byte[] payload = Encoding.UTF8.GetBytes($"{id}|{issuedAt}");
        using HMACSHA256 hmac = new(_secret);
        return Convert.ToBase64String(hmac.ComputeHash(payload));
    }

    private static bool ConstantTimeEquals(string a, string b)
    {
        if (a.Length != b.Length)
            return false;

        return CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(a), Encoding.UTF8.GetBytes(b));
    }
}
