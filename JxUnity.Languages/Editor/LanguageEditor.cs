using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace JxUnity.Languages
{
    internal class LanguagesEditor
    {
        internal static string GeneratedPath = LanguageManager.ReadPath;

        internal static string GetFilePath(string name) => $"{GeneratedPath}/{name}.xml";

        private static void PreparePath(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        [MenuItem("JxLanguages/GenAllTemplate")]
        public static void GenAllTemplate()
        {
            PreparePath(GeneratedPath);

            foreach (LanguageInfo info in LanguageManager.LangInfos)
            {
                var table = new LanguageTable()
                {
                    DisplayName = info.DisplayName,
                    Author = PlayerSettings.companyName,
                    Version = "0.0.0",
                    Time = DateTime.Now.ToString(),
                    Records = new Dictionary<string, string>()
                    {
                        {"sys_langName", info.DisplayName },
                    }
                };
                var path = GetFilePath(info.Name);
                table.SerializeAndSave(path);
            };

            AssetDatabase.Refresh();

            Debug.Log("completed!");
        }
    }

    internal class BuildResult
    {
        public BuildTarget platform;
        public string outFolder;
        public string filenameWithoutExt;
        public string outputFilename;
    }

    internal class LocalizedE : IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPostprocessBuild(BuildReport report)
        {
            BuildResult buildResult = new BuildResult()
            {
                platform = report.summary.platform,
                outFolder = Path.GetDirectoryName(report.summary.outputPath),
                filenameWithoutExt = Path.GetFileNameWithoutExtension(report.summary.outputPath),
                outputFilename = report.summary.outputPath
            };

            var platform = report.summary.platform;

            if (platform == BuildTarget.StandaloneWindows ||
                platform == BuildTarget.StandaloneWindows64)
            {
                Windows(report, buildResult);
            }
        }

        private void Windows(BuildReport report, BuildResult result)
        {
            var readPath = result.outFolder + "/" + LanguageManager.FolderName;

            var srcPath = $"{result.outFolder}/{result.filenameWithoutExt}_Data/StreamingAssets/{LanguageManager.FolderName}";

            if (Directory.Exists(readPath))
            {
                Directory.Delete(readPath, true);
            }
            Debug.Log($"[JxLanguage]: move {srcPath} to {readPath}");
            Directory.Move(srcPath, readPath);
            Debug.Log("[JxLanguage]: build completed!");
        }

    }
}
