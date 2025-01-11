using Autofac;
using Campaign.Infrastructure.Utils.Extensions;
using Extensions.Http.Mvc;

namespace Campaign.Api;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        ServiceExtensions.Init(Configuration);
        services.AddHttpContextAccessor();
        services.AddControllersInternal();
        services.AddExecutionContext();
        services.AddSwaggerInternal();
        services.AddDbContextInternal();
        services.AddAuthInternal();
        services.AddMemoryCache();
        services.AddDistributedMemoryCache();
        services.AddRedisMemoryCache(Configuration);
        services.AddLocalizationInternal();
        services.AddRefitInternal();
        services.AddHostedServicesInternal();
        services.AddPrometheusInternal();
    }

    public void ConfigureContainer(ContainerBuilder builder)
    {
        builder.AddCommandQueryInternal();
        builder.AddKafkaInternal();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {

        app.UseLocalizationInternal();
        app.UsePrometheusInternal();

        app.UseAuthentication();
        app.UseDeveloperExceptionPage();
        app.UseCorsInternal();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpointsInternal(env);
        app.UseSwaggerInternal(env);
    }
}