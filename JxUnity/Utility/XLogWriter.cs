using System;
using System.IO;
using UnityEngine;

/// <summary>
/// 日志文件记录器
/// </summary>
public static class XLogWriter
{
    public static bool IsOutFile { get; set; } = false;
    public static string SaveFolder { get; private set; }
    public static string SaveFileFullName { get; private set; }
    public static string SaveFileName { get; private set; }
    public static int BufferCount { get; private set; }
    private static int bufferCount = 0;
    private static Stream stream = null;
    private static StreamWriter sw = null;

    private static string GetDefaultLogFolder()
    {
        return GetWritablePath() + "/Log";
    }

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

    [Flags]
    public enum OutState
    {
        Editor = 1,
        DebugBuild = 1 << 1,
        ReleaseBuild = 1 << 2,
    }

    public static void Initialize(
        string outFolder = null,
        int bufferCount = 5,
        OutState state = OutState.DebugBuild | OutState.ReleaseBuild)
    {
        if ((state.HasFlag(OutState.Editor) && Application.isEditor)
            || (state.HasFlag(OutState.DebugBuild) && Debug.isDebugBuild)
            || (state.HasFlag(OutState.ReleaseBuild) && !Debug.isDebugBuild)
            )
        {
            if (outFolder == null)
            {
                outFolder = GetDefaultLogFolder();
            }
            Init(outFolder, bufferCount);
        }
    }

    private static void Init(string outFolder, int bufferCount)
    {
        IsOutFile = true;

        SaveFolder = outFolder;
        if (!Directory.Exists(SaveFolder))
            Directory.CreateDirectory(SaveFolder);

        SaveFileName = string.Format("xlw{0}.log", DateTime.Now.ToString("yyyyMMddhhmmss"));
        SaveFileFullName = Path.Combine(SaveFolder, SaveFileName);
        BufferCount = bufferCount;

        stream = File.Open(SaveFileFullName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
        sw = new StreamWriter(stream);

        Application.logMessageReceived += Application_logMessageReceived;
        Application.quitting += Application_quitting;

    }

    private static void Application_quitting()
    {
        if (sw != null)
        {
            sw.Flush();
            stream.Dispose();
            stream = null;
            sw = null;
            Application.logMessageReceived -= Application_logMessageReceived;
        }
    }

    private static string Now => DateTime.Now.ToString("HH:mm:ss");

    private static void Application_logMessageReceived(string condition, string stackTrace, LogType type)
    {
        string log = string.Format("[{0}][{1}] {2}", Now, type.ToString(), condition);
        sw.WriteLine(log);
        switch (type)
        {
            case LogType.Error:
            case LogType.Assert:
            case LogType.Exception:
                sw.WriteLine("\t" + stackTrace.Replace("\n", "\n\t"));
                break;
        }
        bufferCount++;
        if (bufferCount >= BufferCount)
        {
            sw.Flush();
        }
    }

}
