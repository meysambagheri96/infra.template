using System.Threading.Channels;

namespace Campaign.Infrastructure.Utils.NET.LifeTimeScopeRunner;

public class LifetimeScopeRunnerBackgroundService : BackgroundService
{
    private readonly ILifetimeScope _scope;
    private readonly Channel<Func<ILifetimeScope, Task>> _channel;
    private readonly ILogger<LifetimeScopeRunnerBackgroundService> _logger;

    public LifetimeScopeRunnerBackgroundService(ILifetimeScope scope, ILogger<LifetimeScopeRunnerBackgroundService> logger)
    {
        _channel = Channel.CreateUnbounded<Func<ILifetimeScope, Task>>();
        _scope = scope;
        _logger = logger;
    }

    public void Post(Func<ILifetimeScope, Task> backgroundTask)
    {
        _channel.Writer.TryWrite(backgroundTask);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var reader = _channel.Reader;

        while (true)
        {
            var func = await reader.ReadAsync(stoppingToken);
            Run(func);
        }
    }

    private async void Run(Func<ILifetimeScope, Task> func)
    {
        try
        {
            var childScope = _scope.BeginLifetimeScope();
            await func(childScope);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
    }
}