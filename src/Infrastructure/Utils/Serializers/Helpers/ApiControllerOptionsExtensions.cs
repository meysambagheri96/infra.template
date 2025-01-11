using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Campaign.Infrastructure.Utils.Serializers.Helpers;

public static class ApiControllerOptionsExtensions
{
    public static void AddCustomJsonOptions(this IMvcBuilder mvcBuilder, JsonFormatter formatter = JsonFormatter.NewtonsoftJson)
    {
        switch (formatter)
        {
            case JsonFormatter.NewtonsoftJson:
                mvcBuilder.AddNewtonsoftJson(options => options.SetCustomJsonSettings());
                break;
            
            case JsonFormatter.SystemTextJson:
                mvcBuilder.AddJsonOptions(options => options.SetCustomJsonSettings());
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(formatter), formatter, null);
        }
    }
    
    public static void SetCustomJsonSettings(this MvcNewtonsoftJsonOptions options)
    {
        options.SerializerSettings.Converters =
            CustomJsonSerializer.Settings.Converters;

        options.SerializerSettings.ContractResolver =
            CustomJsonSerializer.Settings.ContractResolver;

        options.SerializerSettings.ConstructorHandling =
            CustomJsonSerializer.Settings.ConstructorHandling;

        options.SerializerSettings.ReferenceLoopHandling =
            CustomJsonSerializer.Settings.ReferenceLoopHandling;

        options.SerializerSettings.PreserveReferencesHandling =
            CustomJsonSerializer.Settings.PreserveReferencesHandling;

        options.SerializerSettings.ConstructorHandling =
            CustomJsonSerializer.Settings.ConstructorHandling;

        options.SerializerSettings.ObjectCreationHandling =
            CustomJsonSerializer.Settings.ObjectCreationHandling;
    }

    public static void SetCustomJsonSettings(this JsonOptions options)
    {
        if (!options.JsonSerializerOptions.Converters.Any())
        {
            foreach (var jsonConverter in CustomJsonSerializer.SystemTextJsonOptions.Converters)
            {
                options.JsonSerializerOptions.Converters.Add(jsonConverter);
            }
        }

        options.JsonSerializerOptions.PropertyNameCaseInsensitive =
            CustomJsonSerializer.SystemTextJsonOptions.PropertyNameCaseInsensitive;

        options.JsonSerializerOptions.PropertyNamingPolicy =
            CustomJsonSerializer.SystemTextJsonOptions.PropertyNamingPolicy;

        options.JsonSerializerOptions.Encoder =
            CustomJsonSerializer.SystemTextJsonOptions.Encoder;
    }
}