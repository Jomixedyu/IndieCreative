
using JxUnity.Resources;
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

    private bool isCopyToStreamingAssets;
    private string outputPath;

    private bool isClearTargetFolder;

    private bool platformAndroid;
    private bool platformIOS;
    private bool platformWindows = true;
    private bool platformWindows64;
    private bool platformOSX;
    private bool platformLinux;

    private bool PlatformAndroid { get => platformAndroid; set { if (value) RefreshPlatform(); platformAndroid = value; } }
    private bool PlatformIOS { get => platformIOS; set { if (value) RefreshPlatform(); platformIOS = value; } }
    private bool PlatformWindows { get => platformWindows; set { if (value) RefreshPlatform(); platformWindows = value; } }
    private bool PlatformWindows64 { get => platformWindows64; set { if (value) RefreshPlatform(); platformWindows64 = value; } }
    private bool PlatformOSX { get => platformOSX; set { if (value) RefreshPlatform(); platformOSX = value; } }
    private bool PlatformLinux { get => platformLinux; set { if (value) RefreshPlatform(); platformLinux = value; } }
    private void RefreshPlatform()
    {
        platformAndroid = false;
        platformIOS = false;
        platformWindows = false;
        platformWindows64 = false;
        platformOSX = false;
        platformLinux = false;
    }

    private void OnGUI()
    {
        GUILayout.Label("output path");
        this.outputPath = GUILayout.TextField(this.outputPath);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("StreamingAssets/ResourcePackage"))
        {
            this.outputPath = Application.streamingAssetsPath + "/ResourcePackage";
        }
        if (GUILayout.Button("root/ResourcePackage"))
        {
            this.outputPath = AssetNameUtility.GetRootPath() + "/ResourcePackage";
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(20);
        GUILayout.Label("build target");

        this.PlatformAndroid = GUILayout.Toggle(this.PlatformAndroid, "Android");
        this.PlatformIOS = GUILayout.Toggle(this.PlatformIOS, "iOS");
        this.PlatformWindows = GUILayout.Toggle(this.PlatformWindows, "Windows");
        this.PlatformWindows64 = GUILayout.Toggle(this.PlatformWindows64, "Windows64");
        this.PlatformOSX = GUILayout.Toggle(this.PlatformOSX, "OSX(intel 64)");
        this.PlatformLinux = GUILayout.Toggle(this.PlatformLinux, "Linux64");

        GUILayout.Space(20);
        this.isClearTargetFolder = GUILayout.Toggle(this.isClearTargetFolder, "clear target folder");

        if (GUILayout.Button("Build")) this.Build_Click();
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

        BuildTarget buildTarget = BuildTarget.NoTarget;
        if (this.platformWindows)
            buildTarget = BuildTarget.StandaloneWindows;
        else if (this.platformWindows64)
            buildTarget = BuildTarget.StandaloneWindows64;
        else if (this.platformAndroid)
            buildTarget = BuildTarget.Android;
        else if (this.platformIOS)
            buildTarget = BuildTarget.iOS;
        else if (this.platformOSX)
            buildTarget = BuildTarget.StandaloneOSX;
        else if (this.platformLinux)
            buildTarget = BuildTarget.StandaloneLinux64;

        if (buildTarget == BuildTarget.NoTarget)
        {
            EditorUtility.DisplayDialog("error", "no build target!", "ok");
            return;
        }

        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None, buildTarget);
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("notice", "done", "ok");
    }
}