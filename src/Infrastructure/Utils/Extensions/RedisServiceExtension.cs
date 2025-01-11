using Campaign.Infrastructure.Utils.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Campaign.Infrastructure.Utils.Extensions;

public static class RedisServiceExtension
{
    public static void AddRedisMemoryCache(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService<RedisInitializerHostedService>();

        services.AddStackExchangeRedisCache(o =>
        {
            o.ConnectionMultiplexerFactory = () => RedisConnectionHolder.GetInstance(configuration);
        });

        services.AddSingleton<IRedisConnectionAccessor>(new RedisConnectionAccessor(configuration));
    }
}