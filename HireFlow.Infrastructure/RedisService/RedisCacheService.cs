using System.Text.Json;
using HireFlow.Domain.Interfaces.InterfaceOfService;
using Microsoft.Extensions.Caching.Distributed;

namespace HireFlow.Infrastructure.RedisService;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;

    public RedisCacheService(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var cachedData = await _distributedCache.GetStringAsync(key);
        
        if (string.IsNullOrEmpty(cachedData))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(cachedData);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expirationTime = null)
    {
        var options = new DistributedCacheEntryOptions();

        if (expirationTime.HasValue)
        {
            options.AbsoluteExpirationRelativeToNow = expirationTime.Value;
        }
        else
        {
           
            options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
        }

        var serializedData = JsonSerializer.Serialize(value);
        await _distributedCache.SetStringAsync(key, serializedData, options);
    }

    public async Task RemoveAsync(string key)
    {
        await _distributedCache.RemoveAsync(key);
    }


    public async Task<bool> ExistsAsync(string key)
    {
        var cachedData = await _distributedCache.GetStringAsync(key);
        return !string.IsNullOrEmpty(cachedData);
    }
    
}