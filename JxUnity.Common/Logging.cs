using System;
using System.IO;
using UnityEngine;

public static class Logging
{
    public static string SaveFolder { get; private set; }
    public static string SaveFileFullName { get; private set; }
    public static string SaveFileName { get; private set; }
    public static bool IsOutEditor { get; set; }

    private static bool isOutFile = false;
    public static bool IsOutFile
    {
        get => isOutFile; 
        set
        {
            if (value == false && isOutFile == false) return;
            isOutFile = value;
            if (value) Close(); else OpenStream();
        }
    }

    private static string _logTitle;
    private static uint _cacheSize;
    public static uint CacheSize { get => _cacheSize; set => _cacheSize = value; }
    private static uint logCacheCount = 0;

    private static bool IsRunning { get; set; } = false;

    private static event Action<string> logHandle;
    public static event Action<string> LogHandle
    {
        add { logHandle += value; }
        remove { logHandle -= value; }
    }

    private static StreamWriter sw;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="folderPath">文件路径</param>
    /// <param name="logTitle">日志文件标题</param>
    /// <param name="cacheSize">缓冲大小</param>
    /// <param name="isOutEditor">是否在Unity控制台打印</param>
    public static void Initialize(string folderPath, string logTitle, uint cacheSize = 5, bool isOutEditor = true, bool isOutFile = true)
    {
        if (IsRunning) Close();

        SaveFolder = folderPath;
        IsOutEditor = isOutEditor;
        IsOutFile = isOutFile;

        _logTitle = logTitle;
        _cacheSize = cacheSize;

        if (!Directory.Exists(SaveFolder))
            Directory.CreateDirectory(SaveFolder);

        if (isOutFile) OpenStream();

        IsRunning = true;

        Application.quitting += Application_quitting;
    }

    private static void Application_quitting()
    {
        if(isOutFile)
        {
            Close();
        }
    }

    private static void OpenStream()
    {
        SaveFileName = string.Format("{0}-{1}.log",
            _logTitle, DateTime.Now.ToString("yyyyMMddhhmmss"));

        SaveFileFullName = SaveFolder + "\\" + SaveFileName;

        sw = new StreamWriter(File.OpenWrite(SaveFileFullName));
    }

    public static void Info(object obj)
    {
        if (IsOutEditor)
            Debug.Log(obj);
        string logContent = string.Format("[{0}] INFO: {1}", GetCurTime(), obj);
        Write(logContent);
    }
    public static void Warning(object obj)
    {
        if (IsOutEditor)
            Debug.LogWarning(obj);
        string logContent = string.Format("[{0}] WARNING: {1}", GetCurTime(), obj);
        Write(logContent);
    }
    public static void Error(object obj)
    {
        if (IsOutEditor)
            Debug.LogError(obj);
        string logContent = string.Format("[{0}] ERROR: {1}", GetCurTime(), obj);
        Write(logContent);
    }

    private static void Write(string logContent)
    {
        logHandle?.Invoke(logContent);
        if (!IsOutFile) return;

        logCacheCount++;
        sw.WriteLine(logContent);
        if (logCacheCount >= _cacheSize)
        {
            logCacheCount = 0;
            sw.Flush();
        }
    }

    private static string GetCurTime()
    {
        return DateTime.Now.ToString("hh:mm:ss");
    }

    public static void Close()
    {
        if(sw != null)
        {
            sw.Flush();
            sw.Dispose();
            sw = null;
        }
        IsRunning = false;
    }
}
