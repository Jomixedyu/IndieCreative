using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

/// <summary>
/// 控制Lua脚本的加载
/// </summary>
public class LuaManager : Singleton<LuaManager>
{
    private LuaEnv luaEnv;
    public LuaEnv LuaEnv { get => luaEnv; }

    private Func<string, byte[]> loadFileHandle;

    public LuaManager()
    {
        Initialize("main", (s) => { return System.IO.File.ReadAllBytes("Assets/LuaScripts/" + s + ".lua"); });
        LuaUpdater.GetInstance().OnUpdate += Update;
    }

    public void Initialize(string luaMain, Func<string, byte[]> loadFileHandle)
    {
        this.luaEnv = new LuaEnv();
        this.loadFileHandle = loadFileHandle;
        this.luaEnv.AddLoader(CustomLoader);
        this.luaEnv.DoString(@"print(""LuaEnv Initialize"")");
        this.luaEnv.DoString(string.Format("require (\"{0}\")", luaMain));
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

    private void Update()
    {
        if (this.luaEnv != null)
        {
            this.luaEnv.Tick();
        }
    }

    public override void Dispose()
    {
        this.luaEnv.Dispose();
        base.Dispose();
    }
}