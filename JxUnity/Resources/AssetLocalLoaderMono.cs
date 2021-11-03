using JxUnity.Resources.Private;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JxUnity.Resources
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
        private UnityEngine.Object GetLocalMapItem(string path, Type type)
        {
            AssetBundleMapping.MappingItem item = assetMapping.Mapping(path);
            if (item == null)
            {
                return null;
            }
            string packagepath = AssetNameUtility.UnformatBundleName(item.assetPackageName);
            AssetLocalMap res = UnityEngine.Resources.Load<AssetLocalMap>($"{AssetConfig.LocalRoot}/{packagepath}");

            return res.Get(item.assetName, type);
        }

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
            return this.GetLocalMapItem(path, type);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IEnumerator LoadAsyncCo(string path, Type type, Action<UnityEngine.Object> cb)
        {
            var req = UnityEngine.Resources.LoadAsync(path, type);
            yield return req;
            cb?.Invoke(req.asset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void LoadAsync(string path, Type type, Action<UnityEngine.Object> cb)
        {
            StartCoroutine(LoadAsyncCo(path, type, cb));
        }
    }
}
