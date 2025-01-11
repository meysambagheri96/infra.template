using Autofac;
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
        services.AddSwagger();
        services.AddDbContextInternal();
        services.InjectClasses();
        services.AddAuthInternal();
        services.AddMemoryCache();
        services.AddDistributedMemoryCache();
    }

    public void ConfigureContainer(ContainerBuilder builder)
    {
        builder.AddCommandQueryInternal();
        builder.AddKafkaInternal();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseAuthentication();
        app.UseDeveloperExceptionPage();
        app.UseCorsInternal();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpointsInternal(env);
        app.UseSwaggerInternal(env);
    }
}