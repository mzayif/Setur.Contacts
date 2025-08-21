using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Setur.Contacts.Domain.Models;

namespace Setur.Contacts.ReportApi.Services;

public class RedisReportCacheService : IReportCacheService
{
    private readonly IDistributedCache _cache;
    private readonly RedisSettings _redisSettings;
    private readonly TimeSpan _expirationTime = TimeSpan.FromHours(24); // 24 saat
    private const string KeyPrefix = "report:";

    public RedisReportCacheService(
        IDistributedCache cache,
        IOptions<RedisSettings> redisSettings)
    {
        _cache = cache;
        _redisSettings = redisSettings.Value;
    }

    public async Task<ReportCacheData?> GetReportAsync(Guid reportId)
    {
        var key = $"{_redisSettings.InstanceName}{KeyPrefix}{reportId}";
        var jsonData = await _cache.GetStringAsync(key);

        if (string.IsNullOrEmpty(jsonData))
            return null;

        return JsonConvert.DeserializeObject<ReportCacheData>(jsonData);
    }

    public async Task SetReportAsync(Guid reportId, ReportCacheData reportData)
    {
        var key = $"{_redisSettings.InstanceName}{KeyPrefix}{reportId}";
        var jsonData = JsonConvert.SerializeObject(reportData);

        await _cache.SetStringAsync(key, jsonData, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _expirationTime
        });
    }

    public async Task<bool> DeleteReportAsync(Guid reportId)
    {
        var key = $"{_redisSettings.InstanceName}{KeyPrefix}{reportId}";
        await _cache.RemoveAsync(key);
        return true;
    }

    public async Task<bool> ExistsAsync(Guid reportId)
    {
        var key = $"{_redisSettings.InstanceName}{KeyPrefix}{reportId}";
        var data = await _cache.GetStringAsync(key);
        return !string.IsNullOrEmpty(data);
    }
}
