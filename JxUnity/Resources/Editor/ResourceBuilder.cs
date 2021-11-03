using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using JxUnity.Resources;
using JxUnity.Resources.Private;

public class ResourceBuilder : Editor
{
    [MenuItem("Assets/ResourcePackage/Set Selected Name")]
    [MenuItem("ResourcePackage/Set Selected Name", false, 0)]
    public static void SetSelectName()
    {
        foreach (string item in Selection.assetGUIDs)
        {
            SetName(AssetDatabase.GUIDToAssetPath(item));
        }
        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/ResourcePackage/Set Selected SubNames")]
    [MenuItem("ResourcePackage/Set Selected SubNames", false, 5)]
    public static void SetSelectSubNames()
    {
        foreach (string item in Selection.assetGUIDs)
        {
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

    [MenuItem("Assets/ResourcePackage/Remove Selected Name")]
    [MenuItem("ResourcePackage/Remove Selected Name", false, 10)]
    public static void RemoveSelectName()
    {
        foreach (string item in Selection.assetGUIDs)
        {
            string rootName = AssetDatabase.GUIDToAssetPath(item);
            ResourceBuilderUtility.RemoveName(rootName);
        }
        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/ResourcePackage/Remove Selected SubNames")]
    [MenuItem("ResourcePackage/Remove Selected SubNames", false, 15)]
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

    [MenuItem("ResourcePackage/Remove All Names", false, 100)]
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

    [MenuItem("ResourcePackage/Remove Unused Names", false, 105)]
    public static void RemoveUnusedNames()
    {
        AssetDatabase.RemoveUnusedAssetBundleNames();
        AssetDatabase.Refresh();
    }

    [MenuItem("ResourcePackage/Check Valid", false, 105)]
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
    [MenuItem("ResourcePackage/Generate Resource Mapping", false, 200)]
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
                sb.Append(abName);
                sb.Append('\n');
            }
        }

        if (!Directory.Exists($"Assets/{AssetConfig.ResourceFolderName}"))
        {
            Directory.CreateDirectory($"Assets/{AssetConfig.ResourceFolderName}");
        }

        string fileROOT = $"Assets/{AssetConfig.ResourceFolderName}/{AssetConfig.MapFilename}";
        File.WriteAllText(fileROOT, sb.ToString());

        AssetDatabase.Refresh();
        SetName(fileROOT);
        AssetDatabase.Refresh();
        stopwatch.Stop();
        Debug.Log($"Resource Mapping Generated! count: {assetCount}, ms: {stopwatch.ElapsedMilliseconds}");
    }


    [MenuItem("ResourcePackage/Generate Local ResObjects", false, 205)]
    private static void GenerateResObjects()
    {
        if (AssetSettingsProvider.GetDefaultLoadMode() != AssetLoadMode.Local)
        {
            Debug.Log("LoadMode is not Local");
            return;
        }
        GenerateResourceMapping();

        const string resdir = "Assets/Resources/LocalResPck";

        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        try
        {
            AssetDatabase.StartAssetEditing();
            foreach (string abName in ResourceBuilderUtility.GetUsedAssetBundleNames())
            {
                AssetLocalMap ser = ScriptableObject.CreateInstance<AssetLocalMap>();
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


    [MenuItem("ResourcePackage/Build Resource Package", false, 210)]
    public static void BuildResourcePackage()
    {
        ResourcePackageBuilderWindow.ShowWindow();
    }


    [MenuItem("ResourcePackage/Load Mode/Local", true)]
    public static bool LoadModeLocalValid()
    {
        return AssetSettingsProvider.GetDefaultLoadMode() != AssetLoadMode.Local;
    }
    [MenuItem("ResourcePackage/Load Mode/Package", true)]
    public static bool LoadModePackageValid()
    {
        return AssetSettingsProvider.GetDefaultLoadMode() != AssetLoadMode.Package;
    }

    [MenuItem("ResourcePackage/Load Mode/Local")]
    public static void LoadModeLocal()
    {
        AssetSettingsProvider.SetDefaultLoadMode(AssetLoadMode.Local);
        Debug.Log("Resources Load Mode: Local");
    }
    [MenuItem("ResourcePackage/Load Mode/Package")]
    public static void LoadModePackage()
    {
        AssetSettingsProvider.SetDefaultLoadMode(AssetLoadMode.Package);
        Debug.Log("Resources Load Mode: Package");
    }

    public static bool IsEditorSimulator
    {
        get
        {
            return EditorPrefs.GetBool("AssetManager.IsEditorSimulator", false);
        }
        set
        {
            EditorPrefs.SetBool("AssetManager.IsEditorSimulator", value);
        }
    }

    [MenuItem("ResourcePackage/Simulator/Enable Editor Simulator", validate = true)]
    public static bool EnableEditorSimulator_Validate()
    {
        return !IsEditorSimulator;
    }
    [MenuItem("ResourcePackage/Simulator/Disable Editor Simulator", validate = true)]
    public static bool DisableEditorSimulator_Validate()
    {
        return IsEditorSimulator;
    }

    [MenuItem("ResourcePackage/Simulator/Enable Editor Simulator")]
    public static void EnableEditorSimulator()
    {
        IsEditorSimulator = true;
        Debug.Log("Enable Editor Simulator!");
    }
    [MenuItem("ResourcePackage/Simulator/Disable Editor Simulator")]
    public static void DisableEditorSimulator()
    {
        IsEditorSimulator = false;
        Debug.Log("Disable Editor Simulator!");
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void StaticInit()
    {
        Debug.Log($"[JxUnity.Resources] LoadMode: {AssetSettingsProvider.GetDefaultLoadMode()}, IsSimulator: {IsEditorSimulator}");
    }
}
