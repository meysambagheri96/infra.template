using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Campaign.Infrastructure.Utils.Serializers.Converters;

public class EnumDisplayNameConverter<T> : JsonConverter<T> where T : struct, System.Enum
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (System.Enum.TryParse<T>(value, out var result))
        {
            return result;
        }

        throw new JsonException($"Invalid value '{value}' for enum {typeof(T)}");
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        var displayName = GetDisplayName(value);
        writer.WriteStringValue(displayName ?? value.ToString());
    }

    private string GetDisplayName(System.Enum enumValue)
    {
        var memberInfo = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault();

        if (memberInfo != null)
        {
            var displayAttribute = memberInfo
                .GetCustomAttributes(typeof(DisplayAttribute), false)
                .Cast<DisplayAttribute>()
                .FirstOrDefault();

            if (displayAttribute != null)
            {
                if (displayAttribute.ResourceType != null && !string.IsNullOrWhiteSpace(displayAttribute.Name))
                {
                    var resourceProperty = displayAttribute.ResourceType
                        .GetProperty(displayAttribute.Name, BindingFlags.Static | BindingFlags.Public);

                    if (resourceProperty != null)
                    {
                        return resourceProperty.GetValue(null)?.ToString();
                    }
                }

                return displayAttribute.Name;
            }
        }

        return enumValue.ToString();
    }
}
