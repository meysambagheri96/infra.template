using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Campaign.Infrastructure.Utils.Redis;

public class RedisConnectionAccessor : IRedisConnectionAccessor
{
    private readonly IConfiguration _configuration;

    public RedisConnectionAccessor(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<IConnectionMultiplexer> GetInstance()
    {
        return RedisConnectionHolder.GetInstance(_configuration);
    }

    public async Task<IServer> GetServer()
    {
        var redisConnectionString = _configuration.GetConnectionString("redis");

        var instance = await GetInstance();

        return instance.GetServer(redisConnectionString.Split(",")[0]);
    }
}