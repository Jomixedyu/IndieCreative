using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

/// <summary>
/// Lua侧客户端管理器，用来Tick和发送信息
/// </summary>
public class LuaClientManager : Singleton<LuaClientManager>
{
    private Action<float, float> update;
    private Action lateUpdate;
    private Action<float> fixedUpdate;

    private LuaTable luaClient;
    public LuaTable LuaClient { get => luaClient; }

    private LuaTable luaUnityEvent;

    public LuaClientManager()
    {
        LuaUpdater.GetInstance().OnUpdate += Update;
        LuaUpdater.GetInstance().OnLateUpdate += LateUpdate;
        LuaUpdater.GetInstance().OnFixedUpdate += FixedUpdate;

        this.luaClient = LuaManager.Instance.LuaEnv.Global.Get<LuaTable>("UnityLuaClient");
        this.luaUnityEvent = luaClient.Get<LuaTable>("UnityEvent");

        this.update = luaUnityEvent.Get<Action<float, float>>("__update");
        this.lateUpdate = luaUnityEvent.Get<Action>("__lateUpdate");
        this.fixedUpdate = luaUnityEvent.Get<Action<float>>("__fixedUpdate");
    }

    private void Update()
    {
        if (update != null)
        {
            update(Time.deltaTime, Time.unscaledDeltaTime);
        }
    }
    private void LateUpdate()
    {
        if (this.lateUpdate != null)
        {
            this.lateUpdate();
        }
    }
    private void FixedUpdate()
    {
        if (this.fixedUpdate != null)
        {
            this.fixedUpdate(Time.fixedDeltaTime);
        }
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
    };
}
#endif