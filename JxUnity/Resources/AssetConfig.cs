using System;
using System.Collections.Generic;
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

    }
}