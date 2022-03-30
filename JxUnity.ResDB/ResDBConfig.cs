using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace JxUnity.ResDB.Private
{
    public static class ResDBConfig
    {
        public const string Variant = "pck";
        public const string LocalRoot = "LocalResDB";
        public const string ResDBFolderName = "ResDB";

        public const string MapFilename = "resdb_info.bytes";
        public const string MapName = "resdb_info";

        public static readonly string BundlePrefix = $"{ResDBFolderName.ToLower()}/";

        public static readonly string WorkingFolderName = "ResourceDB";
        public static readonly string WorkingPath = System.Environment.CurrentDirectory + "/" + WorkingFolderName;

        private static readonly string LoadResourcePath = WorkingPath + "/resdb";
        public static readonly string LoadMappingFile = LoadResourcePath + "/resdb_info.bytes";


        public const string kSettingObjectName = "ResDBSettings";

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

        internal static ResDBSettings resourcePackageSettings;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void UnityStaticConstructor()
        {
            resourcePackageSettings = UnityEngine.Resources.Load<ResDBSettings>(kSettingObjectName);
            if (resourcePackageSettings != null)
            {
                ResDatabase.AssetLoadMode = resourcePackageSettings.LoadMode;
            }

            if (Application.isEditor)
            {
                var editor = Assembly.Load("UnityEditor");
                var db = editor.GetType("UnityEditor.AssetDatabase");
                var load = db.GetMethod("LoadAssetAtPath", new Type[] { typeof(string), typeof(Type) });
                ResDBEditorLoader.LoadAssetAtPath = 
                    (Func<string, Type, UnityEngine.Object>)load.CreateDelegate(typeof(Func<string, Type, UnityEngine.Object>));

                var assetEditor = Assembly.Load("JxUnity.ResDB.Editor");
                var assetType = assetEditor.GetType("ResourceBuilder");
                var assetProp = assetType.GetProperty("IsEditorSimulator", BindingFlags.Public | BindingFlags.Static);
                ResDatabase.IsRealMode = (bool)assetProp.GetValue(null);
            }
            else
            {
                ResDBEditorLoader.LoadAssetAtPath = null;
                ResDatabase.IsRealMode = true;
            }

        }

    }
}