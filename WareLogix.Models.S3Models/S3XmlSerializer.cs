using System.Xml;
using System.Xml.Serialization;

namespace WareLogix.Models.S3Models;

/// <summary>
/// TODO: WareLogix XML library
/// </summary>
public class S3XmlSerializer
{
    private static readonly XmlSerializerNamespaces emptySerializerNamespaces = new([XmlQualifiedName.Empty]);

    const string S3XmlNamespace = "http://s3.amazonaws.com/doc/2006-03-01/";

    public static string ToXml<T>(T sourceObject) where T : new()
    {
        try
        {
            using StringWriter sw = new();
            var xmlSerializer = new XmlSerializer(typeof(T), defaultNamespace: S3XmlNamespace);
            xmlSerializer.Serialize(sw, sourceObject, emptySerializerNamespaces);
            return sw.ToString();
        }
        catch (Exception)
        {
            return string.Empty;
        }

        //using MemoryStream ms = new();
        //return System.Text.Encoding.UTF8.GetString(ms.ToArray());
    }

    public static T? FromXml<T>(string xml) where T : class
    {
        try
        {
            using var xmlStringReader = new StringReader(xml);
            var xmlSerializer = new XmlSerializer(typeof(T), defaultNamespace: S3XmlNamespace);
            return xmlSerializer.Deserialize(xmlStringReader) as T;
        }
        catch (Exception)
        {
            return default;
        }

        //using MemoryStream ms = new();
        //xmlSerializer.Serialize(ms, typeof(T), defaultSerializerNamespaces);
        //return System.Text.Encoding.UTF8.GetString(ms.ToArray());
    }
}
