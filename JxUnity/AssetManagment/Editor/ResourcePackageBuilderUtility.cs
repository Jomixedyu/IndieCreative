using UnityEditor;

public static class ResourcePackageBuilderUtility
{
    /// <summary>
    /// 取消该文件夹下所有文件夹与资产的ab包（包含当前文件夹）
    /// </summary>
    /// <param name="path"></param>
    public static void RemoveAllInSub(string path)
    {
        var fileList = AssetNameUtility.GetAllSubAssets(path);
        foreach (var assetName in fileList)
        {
            RemoveName(assetName);
        }
        var folderList = AssetNameUtility.GetAllSubFolders(path);
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
        var list = AssetNameUtility.GetAllSubAssets(path);
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
        assetImporter.SetAssetBundleNameAndVariant(abName, variant);
    }
    /// <summary>
    /// 使用默认后缀设置ab包名字
    /// </summary>
    /// <param name="rootName"></param>
    /// <param name="abName"></param>
    public static void SetName(string rootName, string abName)
    {
        SetName(rootName, abName, AssetConfig.Variant);
    }
    /// <summary>
    /// 按路径和默认后缀设置ab包名字
    /// </summary>
    /// <param name="rootName"></param>
    public static void SetName(string rootName)
    {
        SetName(rootName, AssetNameUtility.RootToBundleName(rootName), AssetConfig.Variant);
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
    /// <param name="rootFullName"></param>
    public static void RemoveName(string rootFullName)
    {
        SetName(rootFullName, null, null);
    }
    /// <summary>
    /// 移除所有ab包
    /// </summary>
    public static void RemoveAllNames()
    {
        var used = AssetNameUtility.GetUsedAssetBundleNames();
        foreach (var item in used)
        {
            RemoveName(AssetNameUtility.BundleNameToRootName(item));
        }
    }
}