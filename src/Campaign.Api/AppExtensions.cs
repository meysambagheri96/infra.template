using System.Diagnostics;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Campaign.Api
{
    public static class AppExtensions
    {
        public static void UseStaticFilesInternal(this IApplicationBuilder app, IConfiguration config, IWebHostEnvironment env)
        {
            var basePath = config.GetSection("StorageConfig:basePath")?.Value;

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
                c.SwaggerEndpoint($"{prefix}/swagger/v1/swagger.json", "FronApi V1");
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
            });
        }
    }
}
