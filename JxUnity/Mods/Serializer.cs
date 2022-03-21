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

        public static ModInfo DeserializeFromFile(Stream stream)
        {
            return (ModInfo)serializer.Deserialize(stream);
        }
        public static void SerializeToFile(ModInfo info, Stream stream)
        {
            StreamWriter sw = new StreamWriter(stream, Encoding.Unicode);
            serializer.Serialize(sw, info);
        }
    }
}
