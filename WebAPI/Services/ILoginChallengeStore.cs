namespace WebAPI.Services;

public interface ILoginChallengeStore
{
    Task<PendingLoginChallenge?> GetAsync(string challengeId, CancellationToken cancellationToken = default);

    Task SetAsync(PendingLoginChallenge challenge, TimeSpan ttl, CancellationToken cancellationToken = default);

    Task RemoveAsync(string challengeId, CancellationToken cancellationToken = default);
}
