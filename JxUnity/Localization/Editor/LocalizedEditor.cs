using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace JxUnity.Localization
{
    internal class LocalizedEditor
    {

        private static string GetFilePath(string name) => $"{LocalizationManager.ReadPath}/{name}.xml";

        private static void PreparePath()
        {
            if (!Directory.Exists(LocalizationManager.ReadPath))
            {
                Directory.CreateDirectory(LocalizationManager.ReadPath);
            }
        }

        [MenuItem("JxLocalization/GenExample")]
        public static void GenExample()
        {
            PreparePath();
            var table = new LocalizedTable()
            {
                Author = "JomiXedYu",
                Time = DateTime.Now.ToString(),
                LangRecords = new List<LocalizedRecord>()
                {
                    new LocalizedRecord("exm","helloworld"),
                }
            };
            var content = LocalizedSerializer.Serialize(table);
            string name = "exm";
            File.WriteAllText(GetFilePath(name), content);
        }
    }

    internal class LocalizedE : IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPostprocessBuild(BuildReport report)
        {
            string output = Path.GetDirectoryName(report.summary.outputPath);
            string outputLang = output + "/" + LocalizationManager.FolderName;

            if (Directory.Exists(outputLang))
            {
                Directory.Delete(outputLang, true);
            }

            if (Directory.Exists(LocalizationManager.ReadPath))
            {
                Directory.CreateDirectory(outputLang);

                var files = Directory.GetFiles(LocalizationManager.ReadPath);
                foreach (var file in files)
                {
                    var filename = Path.GetFileName(file);
                    File.Copy(file, $"{outputLang}/{filename}");
                }
                Debug.Log("JxLocalization: build complete!");
            }

        }
    }
}
