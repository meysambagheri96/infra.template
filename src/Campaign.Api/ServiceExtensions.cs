using Autofac;
using Confluent.Kafka;
using Extensions.Http.Mvc.Filters;
using Infra;
using Infra.Common.Decorators;
using Infra.Eevents;
using Infra.EFCore;
using Infra.Events;
using Infra.Events.Kafka;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Campaign.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Encodings.Web;
using Campaign.Infrastructure.Utils.Serializers.Converters;
using Microsoft.AspNetCore.Localization;

namespace Campaign.Api;

public static class ServiceExtensions
{
    public static IConfiguration Configuration { get; private set; }

    public static void Init(IConfiguration config) => Configuration = config;

    public static void AddDbContextInternal(this IServiceCollection services)
    {
        var connectionString = Configuration.GetConnectionString("SqlServer");
        Guard.NotNullOrEmpty(connectionString, nameof(connectionString));

        services.AddDbContext<CampaignDbContext>(options =>
            options.UseSqlServer(connectionString), ServiceLifetime.Scoped);

        services.AddScoped<DbContext, CampaignDbContext>();
    }

    public static void AddAuthInternal(this IServiceCollection services)
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                var authority = Configuration.GetSection("Urls:Identity")?.Value;

                Guard.NotNullOrEmpty(authority, nameof(authority));

                options.Authority = authority;
                options.RequireHttpsMetadata = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = true,
                    RequireExpirationTime = false,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = false
                };
            });

        var scope = "campaign-api";

        services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireClaim("scope", scope)
                .Build();
        });
    }

    public static void AddControllersInternal(this IServiceCollection services)
    {
        services
            .AddControllers(config =>
            {
                config.Filters.Add(typeof(ExceptionHandlerFilter));
            })
            //.AddApplicationPart(typeof(Startup).Assembly) //You can include other api projects controllers here
            .AddJsonOptions(c =>
            {
                c.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
                c.JsonSerializerOptions.Converters.Add(new EnumDisplayNameConverterFactory());
            });
    }
    
    public static void AddHostedServicesInternal(this IServiceCollection services)
    {
    }
    
    public static void AddRefitInternal(this IServiceCollection services)
    {

    }
    
    public static void AddLocalizationInternal(this IServiceCollection services)
    {
        services.AddLocalization(opt => { opt.ResourcesPath = "Resources"; });

        services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new List<System.Globalization.CultureInfo>
            {
                new("ar"),
                new("fa"),
                new("en"),
                new("tr")
            };

            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
            options.RequestCultureProviders = new IRequestCultureProvider[]
            {
                new Infrastructure.Utils.Localization.CustomRequestCultureProvider()
            };
        });
    }

    public static void AddPrometheusInternal(this IServiceCollection services)
    {
        services.AddSingleton<Metrics>();
    }


    public static void AddSwaggerInternal(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Campaign Api", Version = "v1.0" });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }

    public static void AddKafkaInternal(this ContainerBuilder builder)
    {
        var url = Configuration.GetConnectionString("kafka");
        Guard.NotNullOrEmpty(url, "kafka url");

        builder.AddKafka(producer =>
            {
                producer.BootstrapServers = url;
            },
            consumer =>
            {
                consumer.OffsetResetType = AutoOffsetReset.Earliest;
                consumer.GroupId = "campaign";
                consumer.EventAssemblies = new[]
                {
                    //Api
                    typeof(Startup).Assembly
                };
                consumer.BootstrappServers = url;
            });
    }

    public static ContainerBuilder AddCommandQueryInternal(this ContainerBuilder builder)
    {
        var scannedAssemblies = new[]
        {
            typeof(Startup).Assembly,
        };

        builder.Register<IUnitOfWork>(context =>
            {
                var db = context.Resolve<DbContext>();
                var logger = context.Resolve<ILogger<EfUnitOfWork>>();
                var syncEventBus = context.Resolve<SyncEventBus>();
                var eventBus = context.Resolve<IEventBus>();

                return new EfUnitOfWork(db, eventBus, syncEventBus, logger);
            })
            .InstancePerLifetimeScope();

        builder.AddSyncEventHandlers();
        builder.AddCommandQuery(scannedAssemblies: scannedAssemblies);

        return builder;
    }
}