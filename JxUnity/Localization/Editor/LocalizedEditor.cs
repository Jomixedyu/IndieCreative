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
        internal static string GeneratedPath = LocalizationManager.ReadPath;

        internal static string GetFilePath(string name) => $"{GeneratedPath}/{name}.xml";

        private static void PreparePath(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        [MenuItem("JxLocalization/GenExample")]
        public static void GenExample()
        {
            PreparePath(GeneratedPath);
            var table = new LocalizedTable()
            {
                Locale = "zh-CN",
                Author = "JomiXedYu",
                Version = "0.0.0",
                Time = DateTime.Now.ToString(),
                Records = new Dictionary<string, string>()
                {
                    {"sys_hello", "你好" },
                    {"sys_open", "打开" }
                }
            };

            string name = LocalizationManager.GetLocaleDefaultFilename(LocalizationManager.GetSystemLocale());
            table.SerializeAndSave(GetFilePath(name));
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
            var readPath = result.outFolder + "/" + LocalizationManager.FolderName;

            var srcPath = $"{result.outFolder}/{result.filenameWithoutExt}_Data/StreamingAssets/{LocalizationManager.FolderName}";

            if (Directory.Exists(readPath))
            {
                Directory.Delete(readPath, true);
            }
            Debug.Log($"[JxLocalization]: move {srcPath} to {readPath}");
            Directory.Move(srcPath, readPath);
            Debug.Log("[JxLocalization]: build completed!");
        }

    }
}
