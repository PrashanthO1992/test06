using Microsoft.Extensions.Caching.Distributed;
using Rediscache;
using Rediscache.Models;
using StackExchange.Redis;
using System.Text.Json;

public class OrderCacheService
{
    private readonly IDistributedCache _cache;

    public OrderCacheService(
        IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<Product?> GetAsync(int orderId)
    {
        var key = $"order:{orderId}";

        var json =
            await _cache.GetStringAsync(key);

        if (json == null)
        {
            return null;
        }

        return JsonSerializer.Deserialize<Product>(json);
    }

    public async Task SetAsync(Product order)
    {
        var key = $"order:{order.Id}";

        var json =
            JsonSerializer.Serialize(order);

        var options =
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow =
                    TimeSpan.FromMinutes(10)
            };

        await _cache.SetStringAsync(
            key,
            json,
            options);
    }

    public async Task RemoveAsync(int orderId)
    {
        await _cache.RemoveAsync(
            $"order:{orderId}");
    }
}