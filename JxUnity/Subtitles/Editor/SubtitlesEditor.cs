using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Xml;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using JxUnity.Subtitles;
using System.IO;
using System.Xml.Serialization;
using System.Text;

public class SubtitlesEditor : IPostprocessBuildWithReport
{
    public int callbackOrder => 0;

    static string Folder = Application.streamingAssetsPath + "/Subtitles";

    public static void PrepareDir()
    {
        if (!Directory.Exists(Folder))
        {
            Directory.CreateDirectory(Folder);
        }
    }

    [MenuItem("JxSubtitles/GenExample")]
    public static void GenExample()
    {
        PrepareDir();
        string path = Folder + "/zh-CN.xml";
        SubtitlesTable table = new SubtitlesTable()
        {
            Info = new SubtitlesInfo()
            {
                Id = "sys.zh-cn",
                Locale = new SubtitlesLocale("简体中文", "东北"),
                Author = PlayerSettings.companyName,
                Date = System.DateTime.Now.ToString(),
                Version = "0.0.0"
            },
            Records = new Dictionary<string, string>()
            {
                  {"id1", "你好" }
            }
        };
        using (var fs = File.OpenWrite(path))
        {
            SubtitlesManager.Serialize(table, fs);
        }
        AssetDatabase.Refresh();
    }

    [MenuItem("JxSubtitles/GenConfigExample")]
    public static void GenConfigExample()
    {
        PrepareDir();
        string path = Folder + "/Config.xml";
        SubtitlesConfig cfg = new SubtitlesConfig()
        {
            Fallback = "en-US",
            SubtitlesName = new string[]
            {
                "简体中文"
            }
        };

        XmlSerializer xml = new XmlSerializer(typeof(SubtitlesConfig));

        using (var fs = File.OpenWrite(path))
        {
            StreamWriter sw = new StreamWriter(fs, Encoding.Unicode);
            xml.Serialize(sw, cfg);
        }

        AssetDatabase.Refresh();
    }

    public void OnPostprocessBuild(BuildReport report)
    {

    }
}
