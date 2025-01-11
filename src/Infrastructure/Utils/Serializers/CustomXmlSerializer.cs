using System.Xml;

namespace Campaign.Infrastructure.Utils.Serializers;

public static class CustomXmlSerializer
{
    public static string SerializeXml<T>(this T obj) where T : class
    {
        System.Xml.Serialization.XmlSerializer xsSubmit = new System.Xml.Serialization.XmlSerializer(typeof(T));
        using (var sww = new StringWriter())
        {
            using (XmlTextWriter writer = new XmlTextWriter(sww) { Formatting = Formatting.Indented })
            {
                xsSubmit.Serialize(writer, obj);
                return sww.ToString();
            }
        }
    }

    public static T DeserializeXml<T>(this string @this) where T : class
    {
        var reader = XmlReader.Create(@this.Trim().ToStream(), new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Document });
        return new System.Xml.Serialization.XmlSerializer(typeof(T)).Deserialize(reader) as T;
    }

    private static Stream ToStream(this string @this)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(@this);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }
}