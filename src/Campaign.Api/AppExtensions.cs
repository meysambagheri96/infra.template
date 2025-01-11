using System.Diagnostics;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Prometheus;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Campaign.Api;

public static class AppExtensions
{
    public static void UseStaticFilesInternal(this IApplicationBuilder app, IConfiguration config, IWebHostEnvironment env)
    {
        var basePath = config.GetSection("StorageConfig:BasePath")?.Value;

        Guard.NotNullOrEmpty(basePath, nameof(basePath));

        if (!Directory.Exists(basePath) && !env.IsDevelopment())
        {
            throw new DirectoryNotFoundException(basePath);
        }

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(basePath),
            ServeUnknownFileTypes = true
        });
    }

    public static void UseCorsInternal(this IApplicationBuilder app)
    {
        app.UseCors(policy =>
        {
            policy.AllowAnyOrigin();
            policy.AllowAnyMethod();
            policy.AllowAnyHeader();
        });
    }

    public static void UseSwaggerInternal(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsProduction())
            return;

        var prefix = string.Empty;
        if (!env.IsDevelopment() && !Debugger.IsAttached)
            prefix = "/campaign";

        app.UseSwagger(c =>
        {
            c.PreSerializeFilters.Add((swaggerdoc, httpReq) =>
            {
                swaggerdoc.Servers = new List<OpenApiServer>()
                {
                    new OpenApiServer()
                    {
                        Url = $"{(env.IsDevelopment() || Debugger.IsAttached ? "http" : "https")}://{httpReq.Host.Value}{prefix}"
                    }
                };
            });
        });

        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint($"{prefix}/swagger/v1/swagger.json", "Campaign API V1");
            c.DocExpansion(DocExpansion.None);
        });
    }

    public static void UseEndpointsInternal(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            endpoints.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            endpoints.MapDefaultControllerRoute();
        });
    }
  
    public static void UsePrometheusInternal(this IApplicationBuilder app)
    {
        app.UseMetricServer(9100);
    }

    public static void UseLocalizationInternal(this IApplicationBuilder app)
    {
        var localizeOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
        app.UseRequestLocalization(localizeOptions.Value);
    }
}