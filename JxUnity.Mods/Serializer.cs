using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;


namespace JxUnity.Mods
{
    internal class Serializer
    {
        static XmlSerializer serializer = new XmlSerializer(typeof(ModInfo));

        public static ModInfo DeserializeModInfoFromFile(Stream stream)
        {
            return (ModInfo)serializer.Deserialize(stream);
        }
        public static void SerializeModInfoToFile(ModInfo info, Stream stream)
        {
            StreamWriter sw = new StreamWriter(stream, Encoding.Unicode);
            serializer.Serialize(sw, info);
        }
        public static object DeserializeFromFile(string path, Type type)
        {
            XmlSerializer serializer = new XmlSerializer(type);
            using (var fs = File.OpenRead(path))
            {
                return serializer.Deserialize(fs);
            }
        }
        public static T DeserializeFromFile<T>(string path)
        {
            return (T)DeserializeFromFile(path, typeof(T));
        }
    }
}
