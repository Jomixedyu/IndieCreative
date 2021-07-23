using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static partial class AssetNameUtility
{
    private static readonly int AssetsPathLength = "Assets".Length + 1;

    /// <summary>
    /// 将路径格式化为ab包格式的路径
    /// </summary>
    /// <param name="bundleName"></param>
    /// <returns></returns>
    public static string FormatBundleName(string bundleName)
    {
        var name = Path.GetFileName(bundleName);
        int dot = name.LastIndexOf('.');
        if (dot >= 0)
        {
            var carr = name.ToCharArray();
            carr[dot] = '_';
            name = new string(carr);
        }
        var dir = Path.GetDirectoryName(bundleName).Replace('\\', '/');
        var abExt = AssetConfig.Variant != null ? "." + AssetConfig.Variant : string.Empty;
        return dir + "/" + name + abExt;
    }
    /// <summary>
    /// 将ab包格式的路径格式化为普通路径
    /// </summary>
    /// <param name="bundleName"></param>
    /// <returns></returns>
    public static string UnformatBundleName(string bundleName)
    {
        var name = Path.GetFileName(bundleName);
        if (!string.IsNullOrEmpty(AssetConfig.Variant))
        {
            var ext = "." + AssetConfig.Variant;
            if (!name.EndsWith(ext))
            {
                throw new ArgumentException("bundle ext name is not exist");
            }
            name = name.Substring(0, name.Length - ext.Length);
        }
        int dot = name.LastIndexOf('_');
        if (dot >= 0)
        {
            var carr = name.ToCharArray();
            carr[dot] = '.';
            name = new string(carr);
        }
        var dir = Path.GetDirectoryName(bundleName).Replace('\\', '/');
        return dir + "/" + name;
    }

    /// <summary>
    /// 用ab包全名获取去掉ab后缀的路径
    /// </summary>
    /// <param name="bundleName"></param>
    /// <returns></returns>
    public static string GetBundleNameWithoutVariant(string bundleName)
    {
        if (AssetConfig.Variant == null)
        {
            return bundleName;
        }
        string ext = "." + AssetConfig.Variant;
        if (bundleName.EndsWith(ext))
        {
            return bundleName.Substring(0, bundleName.Length - ext.Length);
        }
        return bundleName;
    }

    public static string RootToBundleName(string rootName)
    {
        return RootToAssetsFolderName(rootName).ToLower().Replace('.', '_');
    }

    public static string RootToAssetsFolderName(string rootName)
    {
        if (!rootName.StartsWith("Assets", true, null))
        {
            return rootName;
        }
        return rootName.Substring(AssetsPathLength);
    }

    public static string BundleNameToRootName(string bundleName)
    {
        return "Assets/" + BundleNameToAssetFolderName(bundleName);
    }

    public static string BundleNameToAssetFolderName(string bundleName)
    {
        return UnformatBundleName(bundleName);
    }
    public static string GetShortName(string name)
    {
        return Path.GetFileNameWithoutExtension(name);
    }


#if UNITY_EDITOR

    /// <summary>
    /// FullName返回AssetsFullName
    /// </summary>
    /// <param name="fullName"></param>
    /// <returns></returns>
    public static string FullNameToAssetsFolderName(string fullName)
    {
        if (fullName.Length == Application.dataPath.Length)
        {
            return string.Empty;
        }
        return fullName.Substring(Application.dataPath.Length + 1).Replace('\\', '/');
    }

    /// <summary>
    /// FullName返回rootFullName
    /// </summary>
    /// <param name="fullName"></param>
    /// <returns></returns>
    public static string FullNameToRootName(string fullName)
    {
        string proj = GetRootPath();
        if (proj == fullName)
        {
            return string.Empty;
        }
        return fullName.Substring(proj.Length + 1).Replace('\\', '/');
    }


    /// <summary>
    /// 获取项目文件夹路径
    /// </summary>
    /// <returns></returns>
    public static string GetRootPath()
    {
        return Application.dataPath.Substring(0, Application.dataPath.IndexOf("/Assets"));
    }

    /// <summary>
    /// 返回一个root起始位置的文件夹名字列表
    /// </summary>
    /// <param name="rootFullname"></param>
    /// <returns></returns>
    public static List<string> GetAllSubFolders(string rootFullname)
    {
        List<string> list = new List<string>();
        string fullName = GetRootPath() + "/" + rootFullname;
        GetAllSubFolders(fullName, list);
        List<string> ret = new List<string>();
        foreach (var item in list)
        {
            string fulName = FullNameToRootName(item);
            if (fullName != string.Empty)
            {
                ret.Add(fulName);
            }
        }
        return ret;
    }
    /// <summary>
    /// 返回一个root起始位置的资源名字列表
    /// </summary>
    /// <param name="rootFullname"></param>
    /// <returns></returns>
    public static List<string> GetAllSubAssets(string rootFullname)
    {
        List<string> list = new List<string>();
        string fullName = GetRootPath() + "/" + rootFullname;
        GetAllSubFiles(fullName, list, ResourceExts);
        List<string> ret = new List<string>();
        foreach (var item in list)
        {
            ret.Add(FullNameToRootName(item));
        }
        return ret;
    }
    /// <summary>
    /// 获取所有正在使用的AB包名
    /// </summary>
    /// <returns></returns>
    public static List<string> GetUsedAssetBundleNames()
    {
        var allNames = AssetDatabase.GetAllAssetBundleNames();
        var unusedNames = AssetDatabase.GetUnusedAssetBundleNames();

        List<string> ret = new List<string>(allNames.Length);

        foreach (string item in allNames)
        {
            if (Array.IndexOf(unusedNames, item) == -1)
            {
                ret.Add(item);
            }
        }
        return ret;
    }

    private static readonly string[] ResourceExts = {".prefab", ".fbx", ".obj",
                             ".png", ".jpg", ".dds", ".gif", ".psd", ".tga", ".bmp",
                             ".txt", ".bytes", ".xml", ".csv", ".json",
                            ".controller", ".shader", ".anim", ".unity", ".mat",
                            ".wav", ".mp3", ".ogg",
                            ".ttf",
                             ".shadervariants", ".asset"};

    private static void GetAllSubFolders(string root, List<string> outList)
    {
        DirectoryInfo info = new DirectoryInfo(root);
        outList.Add(info.FullName);
        foreach (var item in info.GetDirectories())
        {
            GetAllSubFolders(item.FullName, outList);
        }
    }

    private static void GetAllSubFiles(string root, List<string> outList, string[] filter)
    {
        DirectoryInfo dirInfo = new DirectoryInfo(root);
        foreach (var fileInfo in dirInfo.GetFiles())
        {
            if (Array.IndexOf(filter, Path.GetExtension(fileInfo.Name)) != -1)
            {
                outList.Add(fileInfo.FullName);
            }
        }
        foreach (var _dirInfo in dirInfo.GetDirectories())
        {
            GetAllSubFiles(_dirInfo.FullName, outList, filter);
        }

    }

#endif
}
