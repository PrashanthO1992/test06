using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Text.Json;

namespace Rediscache.Service
{
    public class RedisCacheService
    {
        private readonly IDistributedCache _redis;

        public RedisCacheService(IDistributedCache cache)
        {
            _redis = cache;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await _redis.GetStringAsync(key);

            if (value == null)
            {
                return default;
            }

            var json = (string)value;
            return JsonSerializer.Deserialize<T>(json!);
        }

        public async Task SetAsync<T>(
            string key,
            T value,
            TimeSpan expiration)
        {
            var json = JsonSerializer.Serialize(value);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            };
            await _redis.SetStringAsync(
                key,
                json,
                options);
        }

        public async Task RemoveAsync(string key)
        {
            await _redis.RemoveAsync(key);
        }
    }
}
