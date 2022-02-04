using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace JxUnity.ResDB.Private
{
    public static class AssetConfig
    {
        public const string Variant = "pck";
        public const string LocalRoot = "LocalResPck";
        public const string ResourceFolderName = "ResPck";

        public const string MapFilename = "respck_info.bytes";
        public const string MapName = "respck_info";

#if UNITY_EDITOR
        public static readonly string LoadBundleRootPath = Path.GetDirectoryName(Application.dataPath) + "/ResPck_win";
#endif
        public static readonly string LoadManifestBundle = LoadBundleRootPath + "/AssetsPackage";
        public static readonly string LoadResourcePath = LoadBundleRootPath + "/respck";
        public static readonly string LoadMappingFile = LoadResourcePath + "/respck_info.bytes";


        public static Dictionary<RuntimePlatform, string> PlatformName = new Dictionary<RuntimePlatform, string>()
        {
            [RuntimePlatform.WindowsEditor] = "win",
            [RuntimePlatform.WindowsPlayer] = "win",
            [RuntimePlatform.OSXEditor] = "osx",
            [RuntimePlatform.OSXEditor] = "osx",
            [RuntimePlatform.LinuxEditor] = "linux",
            [RuntimePlatform.LinuxPlayer] = "linux",
            [RuntimePlatform.Android] = "android",
            [RuntimePlatform.IPhonePlayer] = "ios",
        };

        internal static ResourcePackageSettings resourcePackageSettings;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void StaticAwake()
        {
            resourcePackageSettings = UnityEngine.Resources.Load<ResourcePackageSettings>("ResourcePackageSettings");
            if (resourcePackageSettings != null)
            {
                AssetManager.AssetLoadMode = resourcePackageSettings.DefaultLoadMode;
            }

            if (Application.isEditor)
            {
                var editor = Assembly.Load("UnityEditor");
                var db = editor.GetType("UnityEditor.AssetDatabase");
                var load = db.GetMethod("LoadAssetAtPath", new Type[] { typeof(string), typeof(Type) });
                AssetManager.LoadAssetAtPath = 
                    (Func<string, Type, UnityEngine.Object>)load.CreateDelegate(typeof(Func<string, Type, UnityEngine.Object>));

                var assetEditor = Assembly.Load("JxUnity.Resources.Editor");
                var assetType = assetEditor.GetType("ResourceBuilder");
                var assetProp = assetType.GetProperty("IsEditorSimulator", BindingFlags.Public | BindingFlags.Static);
                AssetManager.IsModeEnabled = (bool)assetProp.GetValue(null);
            }
            else
            {
                AssetManager.LoadAssetAtPath = null;
                AssetManager.IsModeEnabled = true;

                if (AssetManager.AssetLoadMode == AssetLoadMode.Package)
                {
                    _ = AssetPackageLoaderMono.Instance;
                }
            }

        }

    }
}