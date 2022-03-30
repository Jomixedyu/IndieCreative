using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using JxUnity.ResDB;
using JxUnity.ResDB.Private;

public static class ResourceBuilderUtility
{
    #region base IO

    private static readonly string[] ResourceExts = {".prefab", ".fbx", ".obj",
                             ".png", ".jpg", ".dds", ".gif", ".psd", ".tga", ".bmp", ".tif",
                             ".txt", ".bytes", ".xml", ".csv", ".json",
                            ".controller", ".shader", ".anim", ".unity", ".mat",
                            ".wav", ".mp3", ".ogg",
                            ".mp4", ".mov", ".mpg", ".mpeg", ".avi", ".asf",
                            ".ttf",
                             ".shadervariants", ".asset"};

    /// <summary>
    /// 返回一个root起始位置的文件夹名字列表
    /// </summary>
    /// <param name="rootFullname"></param>
    /// <returns></returns>
    public static List<string> GetAllSubFolders(string rootFullname)
    {
        List<string> list = new List<string>();
        string fullName = AssetNameUtility.GetPROJ() + "/" + rootFullname;
        Internal_GetAllSubFolders(fullName, list);
        List<string> ret = new List<string>();
        foreach (var item in list)
        {
            string fulName = AssetNameUtility.FULLToROOT(item);
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
    /// <param name="ROOTname"></param>
    /// <returns></returns>
    public static List<string> GetAllSubAssets(string ROOTname)
    {
        List<string> list = new List<string>();
        string fullName = AssetNameUtility.GetPROJ() + "/" + ROOTname;
        Internal_GetAllSubFiles(fullName, list, ResourceExts);
        List<string> ret = new List<string>();
        foreach (var item in list)
        {
            ret.Add(AssetNameUtility.FULLToROOT(item));
        }
        return ret;
    }

    private static void Internal_GetAllSubFolders(string root, List<string> outList)
    {
        DirectoryInfo info = new DirectoryInfo(root);
        outList.Add(info.FullName);
        foreach (var item in info.GetDirectories())
        {
            Internal_GetAllSubFolders(item.FullName, outList);
        }
    }

    private static void Internal_GetAllSubFiles(string root, List<string> outList, string[] filter)
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
            Internal_GetAllSubFiles(_dirInfo.FullName, outList, filter);
        }

    }
    #endregion

    /// <summary>
    /// 取消该文件夹下所有文件夹与资产的ab包（包含当前文件夹）
    /// </summary>
    /// <param name="path"></param>
    public static void RemoveAllInSub(string path)
    {
        var fileList = ResourceBuilderUtility.GetAllSubAssets(path);
        foreach (var assetName in fileList)
        {
            RemoveName(assetName);
        }
        var folderList = ResourceBuilderUtility.GetAllSubFolders(path);
        foreach (var folderName in folderList)
        {
            RemoveName(folderName);
        }
    }

    /// <summary>
    /// 设置文件夹下所有资产的ab包名
    /// </summary>
    /// <param name="path"></param>
    public static void SetSubNames(string path)
    {
        var list = ResourceBuilderUtility.GetAllSubAssets(path);
        foreach (string asset in list)
        {
            SetName(asset);
        }
    }
    /// <summary>
    /// 重置文件夹下所有资产的ab包名
    /// </summary>
    /// <param name="rootName"></param>
    public static void ResetSubNames(string rootName)
    {
        if (AssetDatabase.IsValidFolder(rootName))
        {
            RemoveAllInSub(rootName);
        }
        //设置所有子文件为ab包
        SetSubNames(rootName);
    }
    /// <summary>
    /// 设置ab包名字
    /// </summary>
    /// <param name="rootName"></param>
    /// <param name="abName"></param>
    /// <param name="variant"></param>
    public static void SetName(string rootName, string abName, string variant)
    {
        AssetImporter assetImporter = AssetImporter.GetAtPath(rootName);
        if (assetImporter != null)
        {
            assetImporter.SetAssetBundleNameAndVariant(abName, variant);
        }
    }
    /// <summary>
    /// 按路径和默认后缀设置ab包名字
    /// </summary>
    /// <param name="rootName"></param>
    public static void SetName(string rootName)
    {
        SetName(rootName, AssetNameUtility.ROOTToBundleName(rootName), ResDBConfig.Variant);
    }
    /// <summary>
    /// 设置ab包名（如果是文件夹：移除文件夹下所有资产包名）
    /// </summary>
    /// <param name="rootName"></param>
    public static void SetNameAndRemoveSub(string rootName)
    {
        //遍历所有子文件取消ab包
        if (AssetDatabase.IsValidFolder(rootName))
        {
            RemoveAllInSub(rootName);
        }

        SetName(rootName);
    }
    /// <summary>
    /// 移除ab包名字
    /// </summary>
    /// <param name="_ROOT"></param>
    public static void RemoveName(string _ROOT)
    {
        SetName(_ROOT, null, null);
    }


    public static List<string> GetResAllAssetBundleNames()
    {
        string prefix = ResDBConfig.ResDBFolderName.ToLower() + "/";
        var it = AssetDatabase.GetAllAssetBundleNames().Where(s => s.StartsWith(prefix));
        return new List<string>(it);
    }

    /// <summary>
    /// 移除所有ab包
    /// </summary>
    public static void RemoveAllNames()
    {
        foreach (var item in GetResAllAssetBundleNames())
        {
            AssetDatabase.RemoveAssetBundleName(item, true);
        }
    }


    /// <summary>
    /// 获取所有正在使用的AB包名
    /// </summary>
    /// <returns></returns>
    public static List<string> GetUsedAssetBundleNames()
    {
        var allNames = GetResAllAssetBundleNames();
        var unusedNames = AssetDatabase.GetUnusedAssetBundleNames();

        List<string> ret = new List<string>(allNames.Count);

        foreach (string item in allNames)
        {
            if (Array.IndexOf(unusedNames, item) == -1)
            {
                ret.Add(item);
            }
        }
        return ret;
    }
}