using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace JxUnity.Localization
{
    public class LocalizedSerializer
    {
        static StringBuilder sb = new StringBuilder();
        public static string Serialize(LocalizedTable table)
        {
            sb.Clear();
            XmlSerializer xs = new XmlSerializer(typeof(LocalizedTable));
            StringWriter sw = new StringWriter(sb);
            xs.Serialize(sw, table);
            sw.Close();
            return sb.ToString();
        }
        public static LocalizedTable Deserialize(string content)
        {
            XmlSerializer xs = new XmlSerializer(typeof(LocalizedTable));
            StringReader sr = new StringReader(content);
            return (LocalizedTable)xs.Deserialize(sr);
        }

        public static void SerializeAndSave(LocalizedTable table, string path)
        {
            var serc = Serialize(table);
            File.WriteAllText(path, serc, Encoding.Unicode);
        }
        public static LocalizedTable LoadAndDeserialize(string path)
        {
            var content = File.ReadAllText(path, Encoding.Unicode);
            return Deserialize(content);
        }
    }
}
