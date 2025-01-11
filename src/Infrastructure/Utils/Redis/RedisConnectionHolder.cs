using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Campaign.Infrastructure.Utils.Redis;

public static class RedisConnectionHolder
{
    private static IConnectionMultiplexer _instance;

    public static Task<IConnectionMultiplexer> GetInstance(IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetConnectionString("redis");

        return GetInstanceByUrl(redisConnectionString);
    }

    private static async Task<IConnectionMultiplexer> GetInstanceByUrl(string redisConnectionString)
    {
        if (_instance is null || !_instance.IsConnected && !_instance.IsConnecting ||
            _instance.Configuration != redisConnectionString)
        {
            if (_instance is not null)
            {
                await _instance.CloseAsync();
            }
            _instance = await ConnectionMultiplexer.ConnectAsync(redisConnectionString);
        }

        return _instance;
    }
}