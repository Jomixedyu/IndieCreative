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
    ResourceFolder,
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

    [MenuItem("AssetManager/LoadMode/State")]
    public static void State()
    {
        Debug.Log("IsEditorSimulator: " + IsEditorSimulator.ToString());
    }
    [MenuItem("AssetManager/LoadMode/EnableEditorSimulator")]
    public static void EnableEditorSimulator()
    {
        IsEditorSimulator = true;
        Debug.Log("EnableEditorSimulator");
    }
    [MenuItem("AssetManager/LoadMode/DisableEditorSimulator")]
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
            throw new ArgumentException("mapping not found: " + path);
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
            //资源目录
            case AssetLoadMode.ResourceFolder:
#if UNITY_EDITOR
                return AssetDatabase.LoadAssetAtPath<T>("Assets/Resources/" + path);
#else
                //去掉后缀名
                string dirName = Path.GetDirectoryName(path);
                string fileName = Path.GetFileNameWithoutExtension(path);

                return Resources.Load<T>(dirName + "/" + fileName);
#endif
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
            case AssetLoadMode.ResourceFolder:
#if UNITY_EDITOR
                cb.Invoke(AssetDatabase.LoadAssetAtPath("Assets/Resources/" + path, type));
#else
                string dirName = Path.GetDirectoryName(path);
                string fileName = Path.GetFileNameWithoutExtension(path);

                ResourceRequest req = Resources.LoadAsync(dirName + "/" + fileName, type);

                req.completed += (o) =>
                {
                    XLogger.Log("AsyncLoadComplete: " + path);
                    _AssetsAsyncCallBack(req);
                };
                _AddAsyncItem(req, cb);
#endif
                break;
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
