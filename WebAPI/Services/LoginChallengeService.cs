using Core.Entities.Concrete;
using Microsoft.Extensions.Options;
using SessionSentinel.Application.Abstractions;
using WebAPI.Options;

namespace WebAPI.Services;

public sealed class LoginChallengeService : ILoginChallengeService
{
    private readonly ISentinelSessionStore _sessionStore;
    private readonly ILoginChallengeStore _challengeStore;
    private readonly ILoginChallengeEmailSender _emailSender;
    private readonly LoginChallengeOptions _options;

    public LoginChallengeService(
        ISentinelSessionStore sessionStore,
        ILoginChallengeStore challengeStore,
        ILoginChallengeEmailSender emailSender,
        IOptions<LoginChallengeOptions> options)
    {
        _sessionStore = sessionStore;
        _challengeStore = challengeStore;
        _emailSender = emailSender;
        _options = options.Value;
    }

    public async Task<LoginChallengeInitiationResult?> CreateIfRequiredAsync(
        User user,
        string ipAddress,
        string fingerprintHash,
        string? userAgent,
        string? language,
        CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled || string.IsNullOrWhiteSpace(fingerprintHash))
        {
            return null;
        }

        var latestSession = await _sessionStore.GetLatestActiveSessionForUserAsync(user.Id.ToString(), cancellationToken);
        if (latestSession is null || string.Equals(latestSession.IpAddress, ipAddress, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var code = Random.Shared.Next(100000, 999999).ToString();
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(_options.ChallengeTtlMinutes);
        var challenge = new PendingLoginChallenge
        {
            ChallengeId = Guid.NewGuid().ToString("N"),
            UserId = user.Id.ToString(),
            Email = user.Email,
            IpAddress = ipAddress,
            FingerprintHash = fingerprintHash,
            UserAgent = userAgent,
            Language = language,
            Code = code,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = expiresAtUtc
        };

        await _challengeStore.SetAsync(
            challenge,
            TimeSpan.FromMinutes(_options.ChallengeTtlMinutes),
            cancellationToken);

        var dispatchResult = await _emailSender.SendAsync(user.Email, code, cancellationToken);
        return new LoginChallengeInitiationResult(
            challenge.ChallengeId,
            expiresAtUtc,
            dispatchResult.DevelopmentCode,
            dispatchResult.Message);
    }

    public async Task<LoginChallengeVerificationResult> VerifyAsync(
        string challengeId,
        string code,
        CancellationToken cancellationToken = default)
    {
        var challenge = await _challengeStore.GetAsync(challengeId, cancellationToken);
        if (challenge is null)
        {
            return new LoginChallengeVerificationResult(false, "Challenge not found or expired.", null);
        }

        if (challenge.ExpiresAtUtc <= DateTime.UtcNow)
        {
            await _challengeStore.RemoveAsync(challengeId, cancellationToken);
            return new LoginChallengeVerificationResult(false, "Challenge expired.", null);
        }

        if (!string.Equals(challenge.Code, code, StringComparison.Ordinal))
        {
            challenge.FailedAttemptCount += 1;
            if (challenge.FailedAttemptCount >= _options.MaxAttempts)
            {
                await _challengeStore.RemoveAsync(challengeId, cancellationToken);
                return new LoginChallengeVerificationResult(false, "Challenge locked after too many attempts.", null);
            }

            await _challengeStore.SetAsync(
                challenge,
                challenge.ExpiresAtUtc - DateTime.UtcNow,
                cancellationToken);

            return new LoginChallengeVerificationResult(false, "Verification code is invalid.", null);
        }

        await _challengeStore.RemoveAsync(challengeId, cancellationToken);
        return new LoginChallengeVerificationResult(true, null, challenge);
    }
}
