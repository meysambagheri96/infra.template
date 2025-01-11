using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Campaign.Infrastructure.Utils.Serializers;

public static class CustomJsonSerializer
{
    private static readonly JsonSerializerSettings _newtonsoftJsonSettings = new JsonSerializerSettings
    {
        //TypeNameHandling = TypeNameHandling.Auto,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        //PreserveReferencesHandling = PreserveReferencesHandling.Objects,
        ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
        ObjectCreationHandling = ObjectCreationHandling.Replace,
        Converters = new List<Newtonsoft.Json.JsonConverter>
        {
            new StringEnumConverter
            {
                NamingStrategy = new DefaultNamingStrategy()
            }
        },
        ContractResolver = new CamelCasePropertyNamesContractResolver
        {
            DefaultMembersSearchFlags =
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Default
        }
    };

    private static readonly JsonSerializerOptions _systemTextJsonOptions = new JsonSerializerOptions
    {
        Converters = { new JsonStringEnumConverter() },
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public static JsonSerializerSettings Settings { get; } = _newtonsoftJsonSettings;
    public static JsonSerializerOptions SystemTextJsonOptions { get; } = _systemTextJsonOptions;

    /// <summary>
    /// Use NewtonsoftJson for non public constructor and setters
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static string SerializeJson(this object obj)
    {
        return JsonConvert.SerializeObject(obj, _newtonsoftJsonSettings);
    }

    /// <summary>
    /// SerializeJson
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="formatter"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static string SerializeJson(this object obj, JsonFormatter formatter = JsonFormatter.NewtonsoftJson)
    {
        return formatter switch
        {
            JsonFormatter.NewtonsoftJson => JsonConvert.SerializeObject(obj, _newtonsoftJsonSettings),
            JsonFormatter.SystemTextJson => System.Text.Json.JsonSerializer.Serialize(obj, _systemTextJsonOptions),
            _ => throw new ArgumentOutOfRangeException(nameof(formatter), formatter, null)
        };
    }
    
    /// <summary>
    /// DeserializeJson
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="json"></param>
    /// <returns></returns>
    public static T DeserializeJson<T>(this string json)
    {
        return JsonConvert.DeserializeObject<T>(json, _newtonsoftJsonSettings);
    }

    /// <summary>
    /// Use NewtonsoftJson for non public constructor and setters
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="json"></param>
    /// <param name="formatter"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static T DeserializeJson<T>(this string json, JsonFormatter formatter = JsonFormatter.NewtonsoftJson)
    {
        return formatter switch
        {
            JsonFormatter.NewtonsoftJson => JsonConvert.DeserializeObject<T>(json, _newtonsoftJsonSettings),
            JsonFormatter.SystemTextJson => System.Text.Json.JsonSerializer.Deserialize<T>(json, _systemTextJsonOptions),
            _ => throw new ArgumentOutOfRangeException(nameof(formatter), formatter, null)
        };
    }

    /// <summary>
    /// Use NewtonsoftJson for non public constructor and setters
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="json"></param>
    /// <param name="obj"></param>
    /// <param name="formatter"></param>
    /// <returns></returns>
    public static bool TryDeserializeJson<T>(this string json, out T obj, JsonFormatter formatter = JsonFormatter.NewtonsoftJson)
    {
        try
        {
            obj = formatter switch
            {
                JsonFormatter.NewtonsoftJson => JsonConvert.DeserializeObject<T>(json, _newtonsoftJsonSettings),
                JsonFormatter.SystemTextJson => System.Text.Json.JsonSerializer.Deserialize<T>(json, _systemTextJsonOptions),
                _ => throw new ArgumentOutOfRangeException(nameof(formatter), formatter, null)
            };
            return true;
        }
        catch
        {
            obj = default;
            return false;
        }
    }

    /// <summary>
    /// Use NewtonsoftJson for non public constructor and setters
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="formatter"></param>
    /// <returns></returns>
    public static T DeepClone<T>(this object obj, JsonFormatter formatter = JsonFormatter.NewtonsoftJson)
    {
        return SerializeJson(obj, formatter).DeserializeJson<T>(formatter);
    }
}