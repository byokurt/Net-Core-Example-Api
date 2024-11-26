using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace DotnetCoreExampleApi.Extensions;

public static class CachingExtensions
{
    public static async Task Set(this IDistributedCache cache, string cacheKey, object data, TimeSpan expiresIn)
    {
        string serializedItem = JsonSerializer.Serialize(data);

        await cache.SetStringAsync(cacheKey, serializedItem, new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = expiresIn
        });
    }

    public static async Task<T> Get<T>(this IDistributedCache cache, string cacheKey)
    {
        string cacheValue = await cache.GetStringAsync(cacheKey);

        if (string.IsNullOrWhiteSpace(cacheValue))
        {
            return default(T);
        }

        T item = JsonSerializer.Deserialize<T>(cacheValue);

        return item;
    }
}

