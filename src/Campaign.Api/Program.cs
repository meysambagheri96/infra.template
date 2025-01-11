using Autofac.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using System.Diagnostics;

namespace Campaign.Api;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .UseSerilog((context, config) =>
            {
                var elasticUrl = context.Configuration.GetConnectionString("Elastic");

                Guard.NotNullOrEmpty(elasticUrl, nameof(elasticUrl));

                config
                    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticUrl))
                    {
                        AutoRegisterTemplate = true,
                        IndexFormat = $"campaigns-{DateTime.UtcNow:yyyy-MM-dd}"
                    })
                    .WriteTo.Console(restrictedToMinimumLevel: Debugger.IsAttached
                        ? LogEventLevel.Information
                        : LogEventLevel.Warning);
            })
            .ConfigureAppConfiguration((context, config) =>
            {
                foreach (var s in config.Sources)
                {
                    if (s is FileConfigurationSource)
                        ((FileConfigurationSource)s).ReloadOnChange = false;
                }
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.UseKestrel(options =>
                {
                    options.Limits.MaxRequestBodySize = long.MaxValue;
                });
            });
}