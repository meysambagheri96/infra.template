using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;

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