using Microsoft.Extensions.Caching.Memory;

namespace WebAPI.Services;

public sealed class MemoryLoginChallengeStore : ILoginChallengeStore
{
    private readonly IMemoryCache _memoryCache;

    public MemoryLoginChallengeStore(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public Task<PendingLoginChallenge?> GetAsync(string challengeId, CancellationToken cancellationToken = default)
    {
        _memoryCache.TryGetValue($"login-challenge:{challengeId}", out PendingLoginChallenge? challenge);
        return Task.FromResult(challenge);
    }

    public Task SetAsync(PendingLoginChallenge challenge, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        _memoryCache.Set(
            $"login-challenge:{challenge.ChallengeId}",
            challenge,
            new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl
            });

        return Task.CompletedTask;
    }

    public Task RemoveAsync(string challengeId, CancellationToken cancellationToken = default)
    {
        _memoryCache.Remove($"login-challenge:{challengeId}");
        return Task.CompletedTask;
    }
}
