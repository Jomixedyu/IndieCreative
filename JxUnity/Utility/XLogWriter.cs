using System;
using System.IO;
using UnityEngine;

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

    /// <summary>
    /// 在编译打包后的游戏状态下写出日志。
    /// </summary>
    /// <param name="outFolder"></param>
    /// <param name="bufferCount"></param>
    public static void InitInPlayMode(string outFolder, int bufferCount = 5)
    {
#if !UNITY_EDITOR
        Init(outFolder, bufferCount);
#endif
    }
    /// <summary>
    /// 在编译打包后的游戏状态并且编译版本不为Development Build的情况下写出日志。
    /// </summary>
    /// <param name="outFolder"></param>
    /// <param name="bufferCount"></param>
    public static void InitInReleasePlayMode(string outFolder, int bufferCount = 5)
    {
#if !UNITY_EDITOR
        if (!Debug.isDebugBuild)
        {
            Init(outFolder, bufferCount);
        }
#endif
    }

    /// <summary>
    /// 写出日志。
    /// </summary>
    /// <param name="outFolder"></param>
    /// <param name="bufferCount"></param>
    public static void Init(string outFolder, int bufferCount = 5)
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
