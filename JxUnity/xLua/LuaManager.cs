using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

/// <summary>
/// 控制Lua脚本的加载
/// </summary>
public class LuaManager : IDisposable
{
    public static LuaManager mainInstance;
    public static LuaManager CreateMainInstance(string luaMain = "main", Func<string, byte[]> loadFileHandle = null)
    {
        if (mainInstance == null)
        {
            //if (loadFileHandle == null)
            //{
            //    loadFileHandle = (s) => System.IO.File.ReadAllBytes("Assets/LuaScripts/" + s + ".lua");
            //}
            mainInstance = new LuaManager(luaMain, loadFileHandle);
        }
        return mainInstance;
    }
    public static LuaManager GetMainInstance()
    {
        return mainInstance;
    }
    public static bool HasMainInstance => mainInstance != null;


    private LuaEnv luaEnv;
    public LuaEnv LuaEnv { get => luaEnv; }

    private Func<string, byte[]> loadFileHandle;

    private Action<float, float> update;
    private Action lateUpdate;
    private Action<float> fixedUpdate;

    private LuaTable luaClient;
    public LuaTable LuaClient { get => luaClient; }

    private LuaTable luaUnityEvent;

    public LuaManager(string luaMain, Func<string, byte[]> loadFileHandle)
    {
        this.luaEnv = new LuaEnv();
        this.loadFileHandle = loadFileHandle;
        this.luaEnv.AddLoader(CustomLoader);
        this.luaEnv.DoString(@"print(""LuaEnv Initialize"")");
        if (luaMain == null)
        {
            Debug.LogWarning("XluaManager: not exist entry");
        }
        else
        {
            this.luaEnv.DoString(string.Format("require (\"{0}\")", luaMain));
        }

        this.luaClient = this.luaEnv.Global.Get<LuaTable>("UnityLuaClient");
        Debug.LogWarning("XluaManager: load fail JxLuaClient");
        this.luaUnityEvent = luaClient?.Get<LuaTable>("UnityEvent");

        this.update = luaUnityEvent?.Get<Action<float, float>>("__update");
        this.lateUpdate = luaUnityEvent?.Get<Action>("__lateUpdate");
        this.fixedUpdate = luaUnityEvent?.Get<Action<float>>("__fixedUpdate");

        LuaUpdater.GetInstance().OnUpdate += this.XLuaManager_OnUpdate;
        LuaUpdater.GetInstance().OnFixedUpdate += this.XLuaManager_OnFixedUpdate;
        LuaUpdater.GetInstance().OnLateUpdate += this.XLuaManager_OnLateUpdate;
    }
    private void XLuaManager_OnUpdate()
    {
        if (this.luaEnv != null)
        {
            this.luaEnv.Tick();
        }
        if (update != null)
        {
            this.update(Time.deltaTime, Time.unscaledDeltaTime);
        }
    }
    private void XLuaManager_OnLateUpdate()
    {
        if (this.lateUpdate != null)
        {
            this.lateUpdate();
        }
    }

    private void XLuaManager_OnFixedUpdate()
    {
        if (this.fixedUpdate != null)
        {
            this.fixedUpdate(Time.fixedDeltaTime);
        }
    }

    private byte[] CustomLoader(ref string filepath)
    {
        filepath = filepath.Replace('.', '/');

        byte[] text = this.loadFileHandle(filepath);
        if (text == null)
            throw new ArgumentException("读取Lua脚本失败");

        return text;
    }

    public void LoadScript(string scriptPath)
    {
        this.luaEnv.DoString(string.Format("require (\"{0}\")", scriptPath));
    }

    public T GetGlobalVar<T>(string name)
    {
        return this.luaEnv.Global.Get<T>(name);
    }

    public void Dispose()
    {
        this.luaEnv.Dispose();
    }
}

#if UNITY_EDITOR
public static class LuaExport_Method
{
    [CSharpCallLua]
    public static List<Type> CSharpCallLua = new List<Type>()
    {
        typeof(Action<float>),
        typeof(Action<float, float>),
        typeof(Func<bool>),
    };
}
#endif