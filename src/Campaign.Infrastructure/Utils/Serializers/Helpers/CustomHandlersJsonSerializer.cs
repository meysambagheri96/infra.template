namespace Campaign.Infrastructure.Utils.Serializers.Helpers;

public class CustomHandlersJsonSerializer : IJsonSerializer
{
    public string Serialize(object input)
    {
        return input.SerializeJson();
    }

    public T Deserialize<T>(string json)
    {
        return json.DeserializeJson<T>();
    }

    public object Deserialize(string json, Type type)
    {
        return json.DeserializeJson<object>();
    }
}