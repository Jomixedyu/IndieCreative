using JxUnity.ResDB.Private;
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace JxUnity.ResDB
{
    internal sealed class ResDBInlineLoaderMono : MonoBehaviour, IResDBLoader
    {
        private static ResDBInlineLoaderMono instance;
        public static ResDBInlineLoaderMono Instance
        {
            get
            {
                if (instance == null)
                {
                    var go = new GameObject($"__m_{nameof(ResDBInlineLoaderMono)}");
                    DontDestroyOnLoad(go);
                    instance = go.AddComponent<ResDBInlineLoaderMono>();
                }
                return instance;
            }
        }

        private AssetBundleMapping assetMapping;

        private void Awake()
        {
            string filename = $"{ResDBConfig.LocalRoot}/{ResDBConfig.ResDBFolderName.ToLower()}/{ResDBConfig.MapFilename}";
            var runtimeMapping = UnityEngine.Resources.Load<ResDBInlineMap>(filename);

            TextAsset x = (TextAsset)runtimeMapping.Get(ResDBConfig.MapName, typeof(TextAsset));
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
            ResDBInlineMap res = UnityEngine.Resources.Load<ResDBInlineMap>($"{ResDBConfig.LocalRoot}/{resPath}");
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
            var req = UnityEngine.Resources.LoadAsync<ResDBInlineMap>($"{ResDBConfig.LocalRoot}/{resPath}");
            yield return req;
            ResDBInlineMap res = req.asset as ResDBInlineMap;
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
