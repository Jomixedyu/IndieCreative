using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class ResourcePackageBuilder : Editor
{
    [MenuItem("Assets/ResourcePackage/Set Selected Name")]
    [MenuItem("ResourcePackage/Set Selected Name", false, 0)]
    public static void SetSelectName()
    {
        foreach (string item in Selection.assetGUIDs)
        {
            SetName(AssetDatabase.GUIDToAssetPath(item));
        }
    }

    [MenuItem("Assets/ResourcePackage/Set Selected SubNames")]
    [MenuItem("ResourcePackage/Set Selected SubNames", false, 5)]
    public static void SetSelectSubNames()
    {
        foreach (string item in Selection.assetGUIDs)
        {
            SetSubNames(AssetDatabase.GUIDToAssetPath(item));
        }
    }

    internal static void SetName(string rootName)
    {
        ResourcePackageBuilderUtility.SetNameAndRemoveSub(rootName);
    }

    public static void SetSubNames(string rootName)
    {
        ResourcePackageBuilderUtility.ResetSubNames(rootName);
    }

    [MenuItem("Assets/ResourcePackage/Remove Selected Name")]
    [MenuItem("ResourcePackage/Remove Selected Name", false, 10)]
    public static void RemoveSelectName()
    {
        foreach (string item in Selection.assetGUIDs)
        {
            string rootName = AssetDatabase.GUIDToAssetPath(item);
            ResourcePackageBuilderUtility.RemoveName(rootName);
        }
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
                ResourcePackageBuilderUtility.RemoveAllInSub(rootFullName);
            }
        }
    }

    [MenuItem("ResourcePackage/Remove All Names", false, 100)]
    public static void RemoveAllNames()
    {
        if (!EditorUtility.DisplayDialog("warn", "remove all ab names", "yes", "no"))
        {
            return;
        }

        ResourcePackageBuilderUtility.RemoveAllNames();

        EditorUtility.DisplayDialog("msg", "done", "ok");
    }

    [MenuItem("ResourcePackage/Remove Unused Names", false, 105)]
    public static void RemoveUnusedNames()
    {
        AssetDatabase.RemoveUnusedAssetBundleNames();
    }

    [MenuItem("ResourcePackage/Check Valid", false, 105)]
    public static void CheckValid()
    {

    }
    private static void GenerateRuntimeMapping(string path, RuntimeMapping mapping)
    {
        if (AssetManager.AssetLoadMode == AssetLoadMode.NormalAsset)
        {
            var assetsFolderName = AssetNameUtility.RootToAssetsFolderName(path).ToLower();
            var assetShortName = Path.GetFileNameWithoutExtension(assetsFolderName);
            var assetObjects = AssetDatabase.LoadAllAssetsAtPath(path);
            if (assetObjects == null)
            {
                throw new ArgumentException("load fail: " + path);
            }
            foreach (UnityEngine.Object item in assetObjects)
            {
                mapping.Add(assetShortName, item);
            }
        }
    }

    private static string GenerateMapping_Internel(string path, string abName, RuntimeMapping mapping)
    {
        var assetsFolderName = AssetNameUtility.RootToAssetsFolderName(path).ToLower();
        var assetShortName = Path.GetFileNameWithoutExtension(assetsFolderName);
        var guid = AssetDatabase.AssetPathToGUID(path);

        GenerateRuntimeMapping(path, mapping);

        return $"{assetsFolderName}:{assetShortName}:{guid}:{abName}";
    }

    [MenuItem("ResourcePackage/Generate Mapping", false, 200)]
    private static void GenerateMapping()
    {
        StringBuilder sb = new StringBuilder();

        string resourceRootName = "Assets/Resources";

        var abList = AssetNameUtility.GetUsedAssetBundleNames();

        foreach (string abName in abList)
        {
            RuntimeMapping ser = ScriptableObject.CreateInstance<RuntimeMapping>();
            string assetPath = AssetNameUtility.BundleNameToRootName(abName);
            if (AssetDatabase.IsValidFolder(assetPath))
            {
                var assetsNames = AssetNameUtility.GetAllSubAssets(assetPath);
                foreach (string assetRootName in assetsNames)
                {
                    string str = GenerateMapping_Internel(assetRootName, abName, ser);
                    sb.AppendLine(str);
                }
            }
            else
            {
                string str = GenerateMapping_Internel(assetPath, abName, ser);
                sb.AppendLine(str);
            }

            if (AssetManager.AssetLoadMode == AssetLoadMode.NormalAsset)
            {
                var objPath = resourceRootName + "/" + AssetNameUtility.RootToAssetsFolderName(assetPath) + ".asset";
                var dirName = Path.GetDirectoryName(objPath);
                if (!Directory.Exists(dirName))
                {
                    Directory.CreateDirectory(dirName);
                }
                AssetDatabase.CreateAsset(ser, objPath);
            }
        }
        var mappingStr = sb.ToString();

        File.WriteAllText("Assets/ResourcePackage/resource_mapping.bytes", mappingStr, Encoding.UTF8);

        AssetDatabase.Refresh();
        string resMappingName = "Assets/ResourcePackage/resource_mapping.bytes";
        SetName(resMappingName);

        if (AssetManager.AssetLoadMode == AssetLoadMode.NormalAsset)
        {

            RuntimeMapping ser = ScriptableObject.CreateInstance<RuntimeMapping>();
            string objPath = resourceRootName + "/resourcepackage/resource_mapping.bytes.asset";
            GenerateRuntimeMapping(resMappingName, ser);
            AssetDatabase.CreateAsset(ser, objPath);
        }

        AssetDatabase.Refresh();

        Debug.Log("Generated");
    }


    [MenuItem("ResourcePackage/Build AssetBundle", false, 205)]
    public static void BuildAssetBundle()
    {
        ResourcePackageBuilderWindow.ShowWindow();
    }


}
