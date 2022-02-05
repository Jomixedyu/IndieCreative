using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace JxUnity.Localization
{
    [Serializable]
    public class LocalizedRecord
    {
        [XmlAttribute]
        public string Id;
        [XmlAttribute]
        public string Text;
        public LocalizedRecord() { }
        public LocalizedRecord(string id, string text)
        {
            this.Id = id;
            this.Text = text;
        }
    }
    [Serializable]
    public class LocalizedTable
    {
        public string Author { get; set; }
        public string Time { get; set; }
        public List<LocalizedRecord> LangRecords { get; set; }
    }
}
