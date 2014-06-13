using System;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Json;


namespace Vinco.Uploader.Util
{
    public static class JsonSerializer
    {
        public static T Deserialize<T>(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("value");
            }
            using (MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(value)))
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(stream);
            }
        }

        public static string SerializeToJsonString(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            using (MemoryStream stream = new MemoryStream())
            {
                var deserializer = new DataContractJsonSerializer(value.GetType());
                deserializer.WriteObject(stream, value);
                stream.Position = 0;
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
