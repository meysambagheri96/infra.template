using Microsoft.Extensions.DependencyInjection;

namespace Campaign.Infrastructure.Utils.NET.LifeTimeScopeRunner;

public static class LifetimeScopeRunnerServiceExtension 
{
    public static void AddLifetimeScopeRunnerInternal(this IServiceCollection services)
    {
        services.AddSingleton<LifetimeScopeRunnerBackgroundService>();
        services.AddHostedService(provider => provider.GetService<LifetimeScopeRunnerBackgroundService>());
    }
}