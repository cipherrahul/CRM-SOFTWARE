using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace CRM.Shared.Infrastructure;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken ct = default);
    Task RemoveAsync(string key, CancellationToken ct = default);
}

public sealed class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    public RedisCacheService(IDistributedCache cache) => _cache = cache;

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        var data = await _cache.GetStringAsync(key, ct);
        return data is null ? default : JsonSerializer.Deserialize<T>(data);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken ct = default)
    {
        var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(10) };
        var data = JsonSerializer.Serialize(value);
        await _cache.SetStringAsync(key, data, options, ct);
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default) => await _cache.RemoveAsync(key, ct);
}
