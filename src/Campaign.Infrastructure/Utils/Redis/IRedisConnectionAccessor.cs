namespace Campaign.Infrastructure.Utils.Redis;

public interface IRedisConnectionAccessor
{
    Task<IConnectionMultiplexer> GetInstance();
    Task<IServer> GetServer();
}