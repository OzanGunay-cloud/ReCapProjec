namespace WebAPI.Services;

public sealed class PendingLoginChallenge
{
    public string ChallengeId { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string IpAddress { get; set; } = string.Empty;

    public string FingerprintHash { get; set; } = string.Empty;

    public string? UserAgent { get; set; }

    public string? Language { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime ExpiresAtUtc { get; set; }

    public string Code { get; set; } = string.Empty;

    public int FailedAttemptCount { get; set; }
}

public sealed record LoginChallengeInitiationResult(
    string ChallengeId,
    DateTime ExpiresAtUtc,
    string? DevelopmentCode,
    string Message);

public sealed record LoginChallengeDispatchResult(
    bool EmailSent,
    string? DevelopmentCode,
    string Message);

public sealed record LoginChallengeVerificationResult(
    bool IsSuccess,
    string? ErrorMessage,
    PendingLoginChallenge? Challenge);
