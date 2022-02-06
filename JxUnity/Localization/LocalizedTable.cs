using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace JxUnity.Localization
{
    public class LocalizedTable
    {
        public string Locale { get; set; }
        public string Author { get; set; }
        public string Version { get; set; }
        public string Time { get; set; }

        public Dictionary<string, string> Records { get; set; }

        private void PutText(XmlDocument doc, XmlElement root, string name, string value)
        {
            var node = doc.CreateElement(name);
            root.AppendChild(node);
            node.AppendChild(doc.CreateTextNode(value));
        }
        private XmlDocument SerializeXml()
        {
            XmlDocument doc = new XmlDocument();

            XmlElement root = doc.CreateElement(nameof(LocalizedTable));
            doc.AppendChild(root);

            this.PutText(doc, root, nameof(Locale), this.Locale);
            this.PutText(doc, root, nameof(Author), this.Author);
            this.PutText(doc, root, nameof(Version), this.Version);
            this.PutText(doc, root, nameof(Time), this.Time);

            var records = doc.CreateElement(nameof(Records));
            root.AppendChild(records);

            foreach (var record in this.Records)
            {
                var r = doc.CreateElement("Record");
                r.SetAttribute("Id", record.Key);
                r.SetAttribute("Text", record.Value);
                records.AppendChild(r);
            }
            return doc;
        }

        public string Serialize()
        {
            var xml = SerializeXml();
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            xml.Save(sw);
            sw.Close();
            return sb.ToString();
        }
        public void SerializeAndSave(string path)
        {
            var xml = SerializeXml();
            var file = File.Open(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            StreamWriter sw = new StreamWriter(file, Encoding.Unicode);
            xml.Save(sw);
            sw.Close();
        }

        private void DeserializeXml(XmlDocument doc)
        {
            var root = doc[nameof(LocalizedTable)];
            this.Locale = root[nameof(Locale)].FirstChild.Value;
            this.Author = root[nameof(Author)].FirstChild.Value;
            this.Version = root[nameof(Version)].FirstChild.Value;
            this.Time = root[nameof(Time)].FirstChild.Value;
            this.Records = new Dictionary<string, string>();
            foreach (XmlNode item in root[nameof(Records)])
            {
                this.Records.Add(item.Attributes["Id"].Value, item.Attributes["Text"].Value);
            }
        }

        public void Deserialize(string content)
        {
            StringReader sr = new StringReader(content);

            XmlDocument doc = new XmlDocument();
            doc.Load(sr);

            DeserializeXml(doc);
        }
        public void OpenAndDeserialize(string path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            DeserializeXml(doc);
        }
    }
}
