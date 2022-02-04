
using JxUnity.ResDB;
using JxUnity.ResDB.Private;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ResourcePackageBuilderWindow : EditorWindow
{
    public static void ShowWindow()
    {
        //EditorWindow win = GetWindowWithRect(
        //    typeof(AssetBundleBuilderWindow),
        //    new Rect(0, 0, 500, 500),
        //    false,
        //    "BuildAssetBundle");
        EditorWindow win = GetWindow(typeof(ResourcePackageBuilderWindow), false, "BuildResourcePackage");
        win.Show();
    }

    private string outputFolder;
    private string outputPath;

    private BuildTarget buildTarget;
    private bool isClearTargetFolder;

    private Dictionary<BuildTarget, string> targetNames;

    private bool buildPass;

    private void OnEnable()
    {
        targetNames = new Dictionary<BuildTarget, string>()
        {
            [BuildTarget.StandaloneWindows] = "win",
            [BuildTarget.StandaloneWindows64] = "win",
            [BuildTarget.StandaloneLinux64] = "linux",
            [BuildTarget.StandaloneOSX] = "osx",
            [BuildTarget.Android] = "android",
            [BuildTarget.iOS] = "ios",
        };

        this.buildTarget = EditorUserBuildSettings.activeBuildTarget;
    }

    private void OnGUI()
    {
        this.buildPass = true;

        GUILayout.Label("output path");
        this.outputFolder = GUILayout.TextField(this.outputFolder);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("StreamingAssets"))
            this.outputFolder = Application.streamingAssetsPath;

        if (GUILayout.Button("%project%"))
            this.outputFolder = AssetNameUtility.GetPROJ();
        GUILayout.EndHorizontal();

        GUILayout.Space(20);

        this.buildTarget = (BuildTarget)EditorGUILayout.EnumPopup("BuildTarget", this.buildTarget);

        GUILayout.Space(5);

        string targetName;
        targetNames.TryGetValue(this.buildTarget, out targetName);

        this.outputPath = null;
        if (targetName != null)
        {
            if (!string.IsNullOrWhiteSpace(this.outputFolder))
            {
                this.outputPath = this.outputFolder + "/" + ResDBConfig.WorkingFolderName;
                GUILayout.Label(this.outputPath);
            }
            else
            {
                GUILayout.Label("ERROR: input output path");
                this.buildPass = false;
            }
        }
        else
        {
            GUILayout.Label("ERROR: build target not supported");
            this.buildPass = false;
        }

        GUILayout.Space(20);

        this.isClearTargetFolder = GUILayout.Toggle(this.isClearTargetFolder, "clear target folder");

        GUILayout.Space(20);

        EditorGUI.BeginDisabledGroup(!this.buildPass);
        if (GUILayout.Button("Build"))
        {
            this.Build_Click();
        }
        EditorGUI.EndDisabledGroup();
    }

    private void Build_Click()
    {
        string path = this.outputPath;

        //文件夹存在并且不为空
        if (Directory.Exists(path)
            && !(Directory.GetFiles(path).Length == 0 && Directory.GetDirectories(path).Length == 0))
        {
            if (this.isClearTargetFolder)
            {
                Directory.Delete(path, true);
            }
            else
            {
                EditorUtility.DisplayDialog("error", "folder exists", "ok");
                return;
            }
        }

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        var option = 
            BuildAssetBundleOptions.ChunkBasedCompression |
            BuildAssetBundleOptions.DeterministicAssetBundle;

        ResourceBuilder.GenerateResourceMapping();

        Stopwatch stopwatch = Stopwatch.StartNew();
        BuildPipeline.BuildAssetBundles(path, option, buildTarget);
        stopwatch.Stop();

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("notice", $"done. time: {stopwatch.ElapsedMilliseconds}", "ok");
    }
}