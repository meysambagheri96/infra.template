using Autofac.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using System.Diagnostics;

namespace Campaign.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        await CreateHostBuilder(args).Build().RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .UseSerilog((context, config) =>
            {
                var elasticUrl = context.Configuration.GetConnectionString("Elastic");

                Guard.NotNullOrEmpty(elasticUrl, nameof(elasticUrl));

                Serilog.Debugging.SelfLog.Enable(Console.Error);

                config
                    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticUrl))
                    {
                        FailureCallback = (@event, exception) => { Console.WriteLine(@event.Exception); },
                        EmitEventFailure = EmitEventFailureHandling.RaiseCallback,
                        AutoRegisterTemplate = true,
                        IndexFormat = $"campaigns-{DateTime.UtcNow:yyyy-MM-dd}"
                    })
                    .WriteTo.Console(restrictedToMinimumLevel: Debugger.IsAttached
                        ? LogEventLevel.Information
                        : LogEventLevel.Warning);
            })
            .ConfigureAppConfiguration((context, config) =>
            {
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