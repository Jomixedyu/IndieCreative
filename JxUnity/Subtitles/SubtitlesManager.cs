using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace JxUnity.Subtitles
{
    internal class SubtitlesRuntimeInfo
    {
        public SubtitlesInfo Info;
        public string FilePath;

    }

    public static class SubtitlesManager
    {
        private static SubtitlesTable currentTable;

        public static SubtitlesLocale CurrentLocale => currentTable?.Info.Locale;
        public static string CurrentId => currentTable?.Info.Id;

        private static Dictionary<string, SubtitlesRuntimeInfo> infos;


        private static void Load(string id)
        {
            var info = infos[id];
            currentTable = ReadTable(info.FilePath);
        }

        public static string Get(string id)
        {
            if (currentTable == null)
            {

                return null;
            }

            if (currentTable.Records.TryGetValue(id, out var result))
            {
                return result;
            }
            return null;
        }

        internal static void RegisterInfo(SubtitlesRuntimeInfo info)
        {
            infos.Add(info.Info.Id, info);
        }



        //启动时扫描所有MOD配置，并加载Subtitles/Config.xml

        private static StringBuilder sb = new StringBuilder(256);

        internal static SubtitlesInfo LoadInfo(Stream stream)
        {
            sb.Clear();
            StreamReader streamReader = new StreamReader(stream, Encoding.Unicode);

            string line = streamReader.ReadLine();

            while (!line.StartsWith("#Data"))
            {
                sb.Append(line);
                line = streamReader.ReadLine();
            }
            StringReader stringReader = new StringReader(sb.ToString());
            XmlSerializer xmlser = new XmlSerializer(typeof(SubtitlesInfo));
            var obj = xmlser.Deserialize(stringReader);

            return (SubtitlesInfo)obj;
        }
        internal static SubtitlesInfo ReadInfo(string path)
        {
            using (var fs = File.OpenRead(path))
            {
                return LoadInfo(fs);
            }
        }
        internal static SubtitlesTable LoadTable(Stream stream)
        {
            var info = LoadInfo(stream);
            StreamReader streamReader = new StreamReader(stream, Encoding.Unicode);
            string line = streamReader.ReadLine();

            XmlDocument doc = new XmlDocument();
            doc.Load(streamReader);

            SubtitlesTable table = new SubtitlesTable() { Info = info };

            var root = doc[nameof(SubtitlesTable)];
            foreach (XmlNode item in root["Records"])
            {
                table.Records.Add(item.Attributes["Id"].Value, item.Attributes["Text"].Value);
            }

            return table;
        }
        internal static SubtitlesTable ReadTable(string path)
        {
            using (var fs = File.OpenRead(path))
            {
                return LoadTable(fs);
            }
        }


        public static void Serialize(SubtitlesTable table, Stream stream)
        {
            StreamWriter sw = new StreamWriter(stream, Encoding.Unicode);

            XmlSerializer xml1 = new XmlSerializer(typeof(SubtitlesInfo));
            xml1.Serialize(sw, table.Info);

            sw.WriteLine(string.Empty);
            sw.WriteLine("#Data");

            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement(nameof(SubtitlesTable));
            doc.AppendChild(root);
            var records = doc.CreateElement("Records");
            root.AppendChild(records);

            foreach (var record in table.Records)
            {
                var r = doc.CreateElement("Record");
                r.SetAttribute("Id", record.Key);
                r.SetAttribute("Text", record.Value);
                records.AppendChild(r);
            }
            doc.Save(sw);
        }


        public static void FakeScanMode()
        {
            string path = System.Environment.CurrentDirectory + "/Mods";
            var modPaths = Directory.GetDirectories(path);

            foreach (var modpath in modPaths)
            {
                var subtDir = modpath + "/Subtitles";

                if (Directory.Exists(subtDir))
                {
                    ScanFolder(subtDir);
                }
            }
        }

        private static SubtitlesConfig LoadConfig(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }
            using (var fs = File.OpenRead(path))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SubtitlesConfig));
                return (SubtitlesConfig)serializer.Deserialize(fs);
            }
        }

        public static void ScanFolder(string folder)
        {
            var cfgpath = folder + "/Config.xml";
            if (!File.Exists(cfgpath))
            {
                return;
            }
            var cfg = LoadConfig(cfgpath);
            if (cfg == null)
            {
                return;
            }

            foreach (var name in cfg.SubtitlesName)
            {
                string subtPath = $"{folder}/{name}.xml";
                var info = ReadInfo(subtPath);
                RegisterInfo(new SubtitlesRuntimeInfo() { Info = info, FilePath = subtPath });
            }
        }

        public static void FakeScanMod()
        {
            //for
            string subid = "";
            string cfgContent = "";
            string spath = "";
            SubtitlesConfig cfg = new SubtitlesConfig();

            foreach (var name in cfg.SubtitlesName)
            {
                using (var fs = File.Open($"{spath}/{name}.xml", FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var info = LoadInfo(fs);
                    infos.Add(subid, info);
                }
            }

        }
    }
}
