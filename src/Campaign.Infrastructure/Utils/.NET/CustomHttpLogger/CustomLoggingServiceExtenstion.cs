using System.Diagnostics;

namespace Campaign.Infrastructure.Utils.NET.CustomHttpLogger;

public static class CustomLoggingServiceExtenstion
{
    public static void AddCustomHttpClientLoggerInternal(this IServiceCollection services, IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment() || environment.IsStaging() || Debugger.IsAttached)
        {
            services.Replace(ServiceDescriptor
                .Singleton<IHttpMessageHandlerBuilderFilter, CustomLoggingHttpMessageHandlerBuilderFilter>());
        }
    }
}