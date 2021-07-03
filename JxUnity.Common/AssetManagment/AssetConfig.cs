using System;
using System.Collections.Generic;
using UnityEngine;

public static class AssetConfig
{
    public static readonly string Variant = "pck";
    public static readonly string ResourceFolderName = "ResourcePackage";

    public static string LoadBundleRootPath = Application.streamingAssetsPath + "/AssetsPackage";
    public static string LoadManifestBundle = LoadBundleRootPath + "/AssetsPackage";
    public static string LoadResourcePath = LoadBundleRootPath + "/resourcepackage";
    public static string LoadMappingFile = LoadResourcePath + "/resource_mapping.bytes";
    public static string LoadMappingAssetName = "resource_mapping";

}

