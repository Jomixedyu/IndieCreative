using JxUnity.ResDB.Private;
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace JxUnity.ResDB
{
    internal sealed class AssetLocalLoaderMono : MonoBehaviour
    {
        private static AssetLocalLoaderMono instance;
        public static AssetLocalLoaderMono Instance
        {
            get
            {
                if (instance == null)
                {
                    var go = new GameObject($"__m_{nameof(AssetLocalLoaderMono)}");
                    DontDestroyOnLoad(go);
                    instance = go.AddComponent<AssetLocalLoaderMono>();
                }
                return instance;
            }
        }

        private AssetBundleMapping assetMapping;

        private void Awake()
        {
            string filename = $"{AssetConfig.LocalRoot}/{AssetConfig.ResourceFolderName.ToLower()}/{AssetConfig.MapFilename}";
            var runtimeMapping = UnityEngine.Resources.Load<AssetLocalMap>(filename);

            TextAsset x = (TextAsset)runtimeMapping.Get(AssetConfig.MapName, typeof(TextAsset));
            assetMapping = new AssetBundleMapping(x.text);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UnityEngine.Object Load(string path, Type type)
        {
            AssetBundleMapping.MappingItem item = assetMapping.Mapping(path);
            if (item == null)
            {
                return null;
            }
            string resPath = AssetNameUtility.UnformatBundleName(item.assetPackageName);
            AssetLocalMap res = UnityEngine.Resources.Load<AssetLocalMap>($"{AssetConfig.LocalRoot}/{resPath}");
            if (res == null)
            {
                return null;
            }
            return res.Get(item.assetName, type);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IEnumerator LoadAsyncCo(string path, Type type, Action<UnityEngine.Object> cb)
        {
            AssetBundleMapping.MappingItem item = assetMapping.Mapping(path);
            if (item == null)
            {
                cb?.Invoke(null);
                yield break;
            }

            string resPath = AssetNameUtility.UnformatBundleName(item.assetPackageName);
            var req = UnityEngine.Resources.LoadAsync<AssetLocalMap>($"{AssetConfig.LocalRoot}/{resPath}");
            yield return req;
            AssetLocalMap res = req.asset as AssetLocalMap;
            if (res == null)
            {
                cb?.Invoke(null);
                yield break;
            }
            cb?.Invoke(res.Get(item.assetName, type));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void LoadAsync(string path, Type type, Action<UnityEngine.Object> cb)
        {
            StartCoroutine(LoadAsyncCo(path, type, cb));
        }
    }
}
