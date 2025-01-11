namespace Campaign.Infrastructure.Utils.Redis;

public class RedisInitializerHostedService : BackgroundService
{
    private readonly IRedisConnectionAccessor _redisAccessor;
    private readonly ILogger<RedisInitializerHostedService> _logger;

    public RedisInitializerHostedService(IRedisConnectionAccessor redisAccessor, ILogger<RedisInitializerHostedService> logger)
    {
        _redisAccessor = redisAccessor;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _redisAccessor.GetInstance();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
    }
}