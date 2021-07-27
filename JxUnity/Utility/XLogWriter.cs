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
        return System.Environment.CurrentDirectory + "/Log";
    }

    /// <summary>
    /// 在编译打包后的游戏状态下写出日志。
    /// </summary>
    /// <param name="outFolder">日志输出文件夹，如不参入，默认参数为'执行目录/Log'</param>
    /// <param name="bufferCount">缓冲区储存数量，满后写出到文件</param>
    public static void InitInPlayMode(string outFolder = null, int bufferCount = 5)
    {
        if (!Application.isEditor)
        {
            if (string.IsNullOrEmpty(outFolder))
            {
                outFolder = GetDefaultLogFolder();
            }
            Init(outFolder, bufferCount);
        }
    }
    /// <summary>
    /// 在编译打包后的游戏状态并且编译版本不为Development Build的情况下写出日志。
    /// </summary>
    /// <param name="outFolder">日志输出文件夹，如不参入，默认参数为'执行目录/Log'</param>
    /// <param name="bufferCount">缓冲区储存数量，满后写出到文件</param>
    public static void InitInReleasePlayMode(string outFolder = null, int bufferCount = 5)
    {
        if (!Application.isEditor)
        {
            if (!Debug.isDebugBuild)
            {
                if (string.IsNullOrEmpty(outFolder))
                {
                    outFolder = GetDefaultLogFolder();
                }
                Init(outFolder, bufferCount);
            }
        }
    }

    /// <summary>
    /// 初始化写出日志。
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
