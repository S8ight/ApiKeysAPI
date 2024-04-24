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

    public async Task<bool> IsRequestAllowed(string apiKey, int rateLimitSecond, int rateLimitMinute, int rateLimitHour,
        int rateLimitDay)
    {
        var db = _redis.GetDatabase();
        var tasks = new List<Task<bool>>();

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
            
            var tran = db.CreateTransaction();
            tran.AddCondition(Condition.KeyExists(key));
            
            _ = tran.SortedSetAddAsync(key, now.ToString(), now);
            _ = tran.SortedSetRemoveRangeByScoreAsync(key, double.NegativeInfinity, now - period.Interval.TotalSeconds);
            var countTask = tran.SortedSetLengthAsync(key);
            _ = tran.KeyExpireAsync(key, period.Interval);
            
            tasks.Add(tran.ExecuteAsync().ContinueWith(t => countTask.Result <= period.Limit));
        }
        
        var results = await Task.WhenAll(tasks);
        return results.All(result => result);
    }

}