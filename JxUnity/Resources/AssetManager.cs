using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Reflection;

using UnityEngine;
using JxUnity.Resources.Private;

namespace JxUnity.Resources
{
    public enum AssetLoadMode
    {
        Local,
        Package,
    }

    public static class AssetManager
    {
        /// <summary>
        /// get loadmode in runtime
        /// </summary>
        public static AssetLoadMode AssetLoadMode { get; internal set; }

        internal static Func<string, Type, UnityEngine.Object> LoadAssetAtPath;
        internal static bool IsModeEnabled;

        private static AssetBundleMapping assetMapping;
        private static UnityEngine.Object GetLocalMapItem(string path, Type type)
        {
            if (assetMapping == null)
            {
                string filename = $"{AssetConfig.LocalRoot}/{AssetConfig.ResourceFolderName.ToLower()}/{AssetConfig.MapFilename}";
                var runtimeMapping = UnityEngine.Resources.Load<AssetLocalMap>(filename);

                TextAsset x = (TextAsset)runtimeMapping.Get(AssetConfig.MapName, typeof(TextAsset));
                assetMapping = new AssetBundleMapping(x.text);
            }
            AssetBundleMapping.MappingItem item = assetMapping.Mapping(path);
            if (item == null)
            {
                return null;
            }
            string packagepath = AssetNameUtility.UnformatBundleName(item.assetPackageName);
            AssetLocalMap res = UnityEngine.Resources.Load<AssetLocalMap>($"{AssetConfig.LocalRoot}/{packagepath}");
            return res.Get(item.assetName, type);
        }

        /// <summary>
        /// 同步模式加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static object Load(string path, Type type)
        {
            if (string.IsNullOrEmpty(path)) return null;
            if (!IsModeEnabled)
            {
                return LoadAssetAtPath("Assets/" + path, type);
            }
            switch (AssetLoadMode)
            {
                //打包资源
                case AssetLoadMode.Package:
                    return AssetBundleManagerMono.Instance.LoadAsset(path, type);

                case AssetLoadMode.Local:
                    return GetLocalMapItem(path, type);
            }
            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Load<T>(string path) where T : UnityEngine.Object
        {
            return Load(path, typeof(T)) as T;
        }

        //public static Dictionary<ResourceRequest, Action<UnityEngine.Object>> assetsAsyncDict
        //    = new Dictionary<ResourceRequest, Action<UnityEngine.Object>>();

        //private static void _AddAsyncItem(ResourceRequest req, Action<UnityEngine.Object> act)
        //{
        //    assetsAsyncDict.Add(req, new Action<UnityEngine.Object>((asset) => { act(asset); }));
        //}
        //private static void _AssetsAsyncCallBack(ResourceRequest id)
        //{
        //    assetsAsyncDict[id].Invoke(id.asset);
        //    assetsAsyncDict.Remove(id);
        //}

        public static void LoadAsync(string path, Type type, Action<UnityEngine.Object> cb)
        {
            if (string.IsNullOrEmpty(path))
            {
                cb.Invoke(null);
                return;
            }
            if (!IsModeEnabled)
            {
                cb.Invoke(LoadAssetAtPath("Assets/" + path, type));
                return;
            }

            switch (AssetLoadMode)
            {
                case AssetLoadMode.Package:
                    AssetBundleManagerMono.Instance.LoadAssetAsync(path, type, cb);
                    break;
                case AssetLoadMode.Local:
                    cb.Invoke(GetLocalMapItem(path, type));
                    break;
            }
        }
    }
}