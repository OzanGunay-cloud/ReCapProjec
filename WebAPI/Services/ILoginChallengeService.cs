using Core.Entities.Concrete;

namespace WebAPI.Services;

public interface ILoginChallengeService
{
    Task<LoginChallengeInitiationResult?> CreateIfRequiredAsync(
        User user,
        string ipAddress,
        string fingerprintHash,
        string? userAgent,
        string? language,
        CancellationToken cancellationToken = default);

    Task<LoginChallengeVerificationResult> VerifyAsync(
        string challengeId,
        string code,
        CancellationToken cancellationToken = default);
}
