using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace JxUnity.Resources
{
    public static class AssetConfig
    {
        public const string Variant = "pck";
        public const string LocalRoot = "LocalResPck";
        public const string ResourceFolderName = "ResPck";

        public static readonly string LoadBundleRootPath = Application.streamingAssetsPath + "/AssetsPackage";
        public static readonly string LoadManifestBundle = LoadBundleRootPath + "/AssetsPackage";
        public static readonly string LoadResourcePath = LoadBundleRootPath + "/respck";
        public static readonly string LoadMappingFile = LoadResourcePath + "/respck_info.bytes";

        public const string MapFilename = "respck_info.bytes";
        public const string MapName = "respck_info";

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
            Debug.Log("Enable Editor Simulator!");
        }
        [MenuItem("ResourcePackage/LoadMode/Disable Editor Simulator")]
        public static void DisableEditorSimulator()
        {
            IsEditorSimulator = false;
            Debug.Log("Disable Editor Simulator!");
        }
#endif
    }
}