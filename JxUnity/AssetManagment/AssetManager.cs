using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


public enum AssetLoadMode
{
    NormalAsset,
    BundlePackage,
}

public static class AssetManager
{
    public static AssetLoadMode AssetLoadMode { get; set; } = AssetLoadMode.NormalAsset;

    #region IsEditorSimulator
#if UNITY_EDITOR

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

    [MenuItem("ResourcePackage/LoadMode/Enable Editor Simulator", validate = true)]
    public static bool EnableEditorSimulator_Validate()
    {
        return !IsEditorSimulator;
    }
    [MenuItem("ResourcePackage/LoadMode/Disable Editor Simulator", validate = true)]
    public static bool DisableEditorSimulator_Validate()
    {
        return IsEditorSimulator;
    }

    [MenuItem("ResourcePackage/LoadMode/Enable Editor Simulator")]
    public static void EnableEditorSimulator()
    {
        IsEditorSimulator = true;
        Debug.Log("EnableEditorSimulator");
    }
    [MenuItem("ResourcePackage/LoadMode/Disable Editor Simulator")]
    public static void DisableEditorSimulator()
    {
        IsEditorSimulator = false;
        Debug.Log("DisableEditorSimulator");
    }
#endif
    #endregion

    private static AssetMapping assetMapping;
    private static UnityEngine.Object GetRuntimeMappingItem(string path, Type type)
    {
        path = path.ToLower();
        if (assetMapping == null)
        {
            var runtimeMapping = Resources.Load<RuntimeMapping>("resourcepackage/resource_mapping.bytes");

            TextAsset x = (TextAsset)runtimeMapping.Get("resource_mapping", typeof(TextAsset));
            assetMapping = new AssetMapping(x.text);
        }
        var item = assetMapping.Mapping(path);
        if(item == null)
        {
            //throw new ArgumentException("mapping not found: " + path);
            return null;
        }
        var packagepath = AssetNameUtility.UnformatBundleName(item.assetPackageName);
        return Resources.Load<RuntimeMapping>(packagepath).Get(item.assetName, type);
    }

    public static T GetAsset<T>(string path) where T : UnityEngine.Object
    {
        if (string.IsNullOrEmpty(path)) return null;
        path = path.Replace("\\", "/");

        switch (AssetLoadMode)
        {
            //打包资源
            case AssetLoadMode.BundlePackage:
#if UNITY_EDITOR
                if (!IsEditorSimulator)
                    return AssetDatabase.LoadAssetAtPath<T>("Assets/" + path);
                else
                    return (T)AssetBundleManager.Instance.LoadAsset(path, typeof(T));
#else
                return (T)AssetBundleManager.Instance.LoadAsset(path, typeof(T));
#endif
            case AssetLoadMode.NormalAsset:
#if UNITY_EDITOR
                if (!IsEditorSimulator)
                {
                    return AssetDatabase.LoadAssetAtPath<T>("Assets/" + path);
                }
                else
                {
                    return (T)GetRuntimeMappingItem(path, typeof(T));
                }
#else
                return (T)GetRuntimeMappingItem(path, typeof(T));
#endif
        }
        return null;
    }

    public static Dictionary<ResourceRequest, Action<UnityEngine.Object>> assetsAsyncDict
        = new Dictionary<ResourceRequest, Action<UnityEngine.Object>>();

    private static void _AddAsyncItem(ResourceRequest req, Action<UnityEngine.Object> act)
    {
        assetsAsyncDict.Add(req, new Action<UnityEngine.Object>((asset) => { act(asset); }));
    }
    private static void _AssetsAsyncCallBack(ResourceRequest id)
    {
        assetsAsyncDict[id].Invoke(id.asset);
        assetsAsyncDict.Remove(id);
    }

    public static void GetAssetAsync(string path, Type type, Action<UnityEngine.Object> cb)
    {
        if (cb == null)
        {
            return;
        }
        if (string.IsNullOrEmpty(path))
        {
            cb.Invoke(null);
            return;
        }
        path = path.Replace("\\", "/");

        switch (AssetLoadMode)
        {
            case AssetLoadMode.BundlePackage:
#if UNITY_EDITOR
                if (!IsEditorSimulator)
                    cb.Invoke(AssetDatabase.LoadAssetAtPath("Assets/" + path, type));
                else
                    AssetBundleManager.Instance.LoadAssetAsync(path, type, cb);
#else
                AssetBundleManager.Instance.LoadAssetAsync(path, type, cb);
#endif
                break;
            case AssetLoadMode.NormalAsset:
#if UNITY_EDITOR
                if (!IsEditorSimulator)
                    cb.Invoke(AssetDatabase.LoadAssetAtPath("Assets/" + path, type));
                else
                    cb.Invoke(GetRuntimeMappingItem(path, type));
#else
                cb.Invoke(GetRuntimeMappingItem(path, type));
#endif
                break;
        }
    }
}
