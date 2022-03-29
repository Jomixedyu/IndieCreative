using System.IO;
using UnityEngine;
using System;

public static class PathHelper
{
    /// <summary>
    /// 可写路径
    /// </summary>
    public static readonly string WritablePath = GetWritablePath();

    public static readonly string Config = $"{WritablePath}/Config";
    public static readonly string SaveData = $"{WritablePath}/SaveData";
    public static readonly string Log = $"{WritablePath}/Log";
    public static readonly string ScreenShot = $"{WritablePath}/ScreenShot";
    public static readonly string Icon = $"{WritablePath}/Icon";
    public static readonly string Mods = $"{WritablePath}/Mods";

    private static string GetWritablePath()
    {
        if (Application.isEditor)
        {
            return System.Environment.CurrentDirectory;
        }
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsPlayer:
                return global::System.AppDomain.CurrentDomain.BaseDirectory;
            case RuntimePlatform.Android:
                return Application.persistentDataPath;
            case RuntimePlatform.IPhonePlayer:
                return Application.temporaryCachePath;
            default:
                break;
        }
        return null;
    }

}
