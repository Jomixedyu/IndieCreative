using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using JxUnity.ResDB;
using JxUnity.ResDB.Private;

public class ResourceBuilder : Editor
{
    [MenuItem("Assets/ResDB/Set Selected Name")]
    [MenuItem("ResDB/Set Selected Name", false, 0)]
    public static void SetSelectName()
    {
        foreach (string item in Selection.assetGUIDs)
        {
            var path = AssetDatabase.GUIDToAssetPath(item);
            var abName = AssetNameUtility.ROOTToBundleName(path);
            if (!abName.StartsWith(ResDBConfig.BundlePrefix))
            {
                Debug.LogError("the file is not in folder ResDB: " + path);
                continue;
            }
            SetName(path);
        }
        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/ResDB/Set Selected SubNames")]
    [MenuItem("ResDB/Set Selected SubNames", false, 5)]
    public static void SetSelectSubNames()
    {
        foreach (string item in Selection.assetGUIDs)
        {
            var path = AssetDatabase.GUIDToAssetPath(item);
            var abName = AssetNameUtility.ROOTToBundleName(path);
            if (!abName.StartsWith(ResDBConfig.BundlePrefix))
            {
                Debug.LogError("the file is not in folder ResDB: " + path);
                continue;
            }
            SetSubNames(AssetDatabase.GUIDToAssetPath(item));
        }
        AssetDatabase.Refresh();
    }

    internal static void SetName(string rootName)
    {
        ResourceBuilderUtility.SetNameAndRemoveSub(rootName);
    }

    internal static void SetSubNames(string rootName)
    {
        ResourceBuilderUtility.ResetSubNames(rootName);
    }

    [MenuItem("Assets/ResDB/Remove Selected Name")]
    [MenuItem("ResDB/Remove Selected Name", false, 10)]
    public static void RemoveSelectName()
    {
        foreach (string item in Selection.assetGUIDs)
        {
            string rootName = AssetDatabase.GUIDToAssetPath(item);
            ResourceBuilderUtility.RemoveName(rootName);
        }
        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/ResDB/Remove Selected SubNames")]
    [MenuItem("ResDB/Remove Selected SubNames", false, 15)]
    public static void RemoveSelectSubNames()
    {
        foreach (string item in Selection.assetGUIDs)
        {
            string rootFullName = AssetDatabase.GUIDToAssetPath(item);
            if (AssetDatabase.IsValidFolder(rootFullName))
            {
                ResourceBuilderUtility.RemoveAllInSub(rootFullName);
            }
        }
        AssetDatabase.Refresh();
    }

    [MenuItem("ResDB/Remove All Names", false, 100)]
    public static void RemoveAllNames()
    {
        if (!EditorUtility.DisplayDialog("warn", "remove all ab names", "yes", "no"))
        {
            return;
        }

        ResourceBuilderUtility.RemoveAllNames();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("msg", "done", "ok");
    }

    [MenuItem("ResDB/Remove Unused Names", false, 105)]
    public static void RemoveUnusedNames()
    {
        AssetDatabase.RemoveUnusedAssetBundleNames();
        AssetDatabase.Refresh();
    }

    [MenuItem("ResDB/Check Valid", false, 105)]
    public static void CheckValid()
    {
        //AssetDatabase.IsValidFolder();

        //foreach (var item in AssetDatabase.GetAssetPathsFromAssetBundle("test_folder.pck"))
        //{
        //    Debug.Log(item);

        //}

    }

    /// <summary>
    /// 生成映射表
    /// </summary>
    [MenuItem("ResDB/Generate Resource Mapping", false, 200)]
    public static void GenerateResourceMapping()
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        StringBuilder sb = new StringBuilder(1024 * 64);

        var abList = ResourceBuilderUtility.GetUsedAssetBundleNames();
        int assetCount = 0;
        foreach (string abName in abList)
        {
            foreach (string assetROOT in AssetDatabase.GetAssetPathsFromAssetBundle(abName))
            {
                ++assetCount;
                sb.Append(AssetNameUtility.ROOTToASSET(assetROOT));
                sb.Append(':');
                sb.Append(Path.GetFileNameWithoutExtension(assetROOT));
                sb.Append(':');
                sb.Append(AssetDatabase.AssetPathToGUID(assetROOT));
                sb.Append(':');
                sb.Append(abName);
                sb.Append('\n');
            }
        }

        if (!Directory.Exists($"Assets/{ResDBConfig.ResDBFolderName}"))
        {
            Directory.CreateDirectory($"Assets/{ResDBConfig.ResDBFolderName}");
        }

        string fileROOT = $"Assets/{ResDBConfig.ResDBFolderName}/{ResDBConfig.MapFilename}";
        File.WriteAllText(fileROOT, sb.ToString());

        AssetDatabase.Refresh();
        SetName(fileROOT);
        AssetDatabase.Refresh();
        stopwatch.Stop();
        Debug.Log($"Resource Mapping Generated! count: {assetCount}, ms: {stopwatch.ElapsedMilliseconds}");
    }

    [MenuItem("ResDB/Generate Local ResObjects", false, 205)]
    private static void GenerateResObjects()
    {
        if (AssetSettingsProvider.GetDefaultLoadMode() != AssetLoadMode.Inline)
        {
            Debug.Log("LoadMode is not Local");
            return;
        }
        GenerateResourceMapping();

        string resdir = $"Assets/Resources/{ResDBConfig.LocalRoot}";

        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        try
        {
            AssetDatabase.StartAssetEditing();
            foreach (string abName in ResourceBuilderUtility.GetUsedAssetBundleNames())
            {
                ResDBInlineMap ser = ScriptableObject.CreateInstance<ResDBInlineMap>();
                foreach (string assetROOT in AssetDatabase.GetAssetPathsFromAssetBundle(abName))
                {
                    var assetObjects = AssetDatabase.LoadAllAssetsAtPath(assetROOT);
                    foreach (UnityEngine.Object item in assetObjects)
                    {
                        ser.Add(item.name, item);
                    }
                }

                string assetsoPath = resdir + "/" + AssetNameUtility.UnformatBundleName(abName) + ".asset";
                string assetsoDir = Path.GetDirectoryName(assetsoPath);
                if (!Directory.Exists(assetsoDir))
                {
                    Directory.CreateDirectory(assetsoDir);
                }

                AssetDatabase.CreateAsset(ser, assetsoPath);
            }
            AssetDatabase.Refresh();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        finally
        {
            stopwatch.Stop();
            AssetDatabase.StopAssetEditing();
        }

        Debug.Log("Local ResObjects Generated! ms: " + stopwatch.ElapsedMilliseconds.ToString());
    }


    [MenuItem("ResDB/Build Resource Package", false, 210)]
    public static void BuildResourcePackage()
    {
        ResourcePackageBuilderWindow.ShowWindow();
    }


    [MenuItem("ResDB/Load Mode/Inline", true)]
    public static bool LoadModeLocalValid()
    {
        return AssetSettingsProvider.GetDefaultLoadMode() != AssetLoadMode.Inline;
    }
    [MenuItem("ResDB/Load Mode/Package", true)]
    public static bool LoadModePackageValid()
    {
        return AssetSettingsProvider.GetDefaultLoadMode() != AssetLoadMode.Package;
    }

    [MenuItem("ResDB/Load Mode/Inline")]
    public static void LoadModeLocal()
    {
        AssetSettingsProvider.SetDefaultLoadMode(AssetLoadMode.Inline);
        Debug.Log("ResDB Load Mode: Inline");
    }
    [MenuItem("ResDB/Load Mode/Package")]
    public static void LoadModePackage()
    {
        AssetSettingsProvider.SetDefaultLoadMode(AssetLoadMode.Package);
        Debug.Log("ResDB Load Mode: Package");
    }

    public static bool IsEditorSimulator
    {
        get
        {
            return EditorPrefs.GetBool("ResDB.IsEditorSimulator", false);
        }
        set
        {
            EditorPrefs.SetBool("ResDB.IsEditorSimulator", value);
        }
    }

    [MenuItem("ResDB/Simulator/Enable Editor Simulator", validate = true)]
    public static bool EnableEditorSimulator_Validate()
    {
        return !IsEditorSimulator;
    }
    [MenuItem("ResDB/Simulator/Disable Editor Simulator", validate = true)]
    public static bool DisableEditorSimulator_Validate()
    {
        return IsEditorSimulator;
    }

    [MenuItem("ResDB/Simulator/Enable Editor Simulator")]
    public static void EnableEditorSimulator()
    {
        IsEditorSimulator = true;
        Debug.Log("Enable Editor Simulator!");
    }
    [MenuItem("ResDB/Simulator/Disable Editor Simulator")]
    public static void DisableEditorSimulator()
    {
        IsEditorSimulator = false;
        Debug.Log("Disable Editor Simulator!");
    }

    [MenuItem("ResDB/Check Dependent")]
    public static void CheckDependent()
    {

    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void StaticInit()
    {
        Debug.Log($"[JxUnity.ResDB] LoadMode: {AssetSettingsProvider.GetDefaultLoadMode()}, IsSimulator: {IsEditorSimulator}");
    }
}
