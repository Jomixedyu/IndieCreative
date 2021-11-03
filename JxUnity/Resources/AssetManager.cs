using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Reflection;

using UnityEngine;
using JxUnity.Resources.Private;
using System.Collections;

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
                    return AssetPackageLoaderMono.Instance.LoadAsset(path, type);

                case AssetLoadMode.Local:
                    return AssetLocalLoaderMono.Instance.Load(path, type);
                default:
                    break;
            }
            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Load<T>(string path) where T : UnityEngine.Object
        {
            return Load(path, typeof(T)) as T;
        }

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
                    AssetPackageLoaderMono.Instance.LoadAssetAsync(path, type, cb);
                    break;
                case AssetLoadMode.Local:
                    AssetLocalLoaderMono.Instance.LoadAsync(path, type, cb);
                    break;
            }
        }
    }
}