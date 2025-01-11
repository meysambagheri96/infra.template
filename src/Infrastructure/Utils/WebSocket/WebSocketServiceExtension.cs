using Microsoft.Extensions.DependencyInjection;

namespace Campaign.Infrastructure.Utils.WebSocket;

public static class WebSocketServiceExtension
{
    public static void AddWebSocketInternal(this IServiceCollection services)
    {
        services.AddScoped<WebSocket>();
    }
}