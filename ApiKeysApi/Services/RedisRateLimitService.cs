using ApiKeysApi.DataAccess.Entities;
using ApiKeysApi.Interfaces;
using StackExchange.Redis;

namespace ApiKeysApi.Services;

public class RedisRateLimitService : IRateLimitService
{
    private readonly IConnectionMultiplexer _redis;

    public RedisRateLimitService(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task<bool> IsRequestAllowed(string apiKey, int rateLimitSecond, int rateLimitMinute, int rateLimitHour, int rateLimitDay)
    {
        var db = _redis.GetDatabase();
        var results = new List<bool>();

        var periods = new[]
        {
            new { Interval = TimeSpan.FromSeconds(1), Limit = rateLimitSecond },
            new { Interval = TimeSpan.FromMinutes(1), Limit = rateLimitMinute },
            new { Interval = TimeSpan.FromHours(1), Limit = rateLimitHour },
            new { Interval = TimeSpan.FromDays(1), Limit = rateLimitDay }
        };

        foreach (var period in periods)
        {
            var key = $"ratelimit:{apiKey}:{period.Interval.TotalSeconds}";
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            if (!await db.KeyExistsAsync(key))
            {
                await db.SortedSetAddAsync(key, "init", double.MinValue);
                await db.KeyExpireAsync(key, period.Interval);
            }

            var tran = db.CreateTransaction();

            _ = tran.SortedSetAddAsync(key, now.ToString(), now);
            _ = tran.SortedSetRemoveRangeByScoreAsync(key, double.NegativeInfinity, now - period.Interval.TotalSeconds);
            var countTask = tran.SortedSetLengthAsync(key);
            _ = tran.KeyExpireAsync(key, period.Interval);

            bool executed = await tran.ExecuteAsync();
            if (!executed)
            {
                results.Add(false);
                continue;
            }

            long currentCount = await countTask;
            results.Add(currentCount <= period.Limit);
        }

        return results.All(result => result);
    }
}


