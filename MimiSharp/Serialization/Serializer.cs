using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MimiSharp.Serialization
{
    public static class Serializer
    {

        public static string? Serialize(object data , string contentType)
        {
            return contentType.ToLower() switch
            {
                "application/json" => SerializeToJson(data),
                "application/xml" => SerializeToXml(data),
                "text/plain" => data.ToString(),
                "text/html" => data.ToString(),
                _ => throw new NotSupportedException($"Content type {contentType} is not supported.")
            };
        }
        public static string? Serialize<T>(T data, string contentType)
        {
            return contentType.ToLower() switch
            {
                "application/json" => SerializeToJson<T>(data),
                "application/xml" => SerializeToXml<T>(data),
                _ => throw new NotSupportedException($"Content type {contentType} is not supported.")
            };
        }
        private static string SerializeToXml(object data)
        {
                var xmlSerializer = new XmlSerializer(typeof(object));
                using var stringWriter = new StringWriter();
                xmlSerializer.Serialize(stringWriter, data);
                return stringWriter.ToString();
        }
        private static string SerializeToXml<T>(T data)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            using var stringWriter = new StringWriter();
            xmlSerializer.Serialize(stringWriter, data);
            return stringWriter.ToString();
        }

        private static string SerializeToJson(object data)
                  => JsonSerializer.Serialize(data);

        private static string SerializeToJson<T>(T data)
              => JsonSerializer.Serialize<T>(data);
    }

}
