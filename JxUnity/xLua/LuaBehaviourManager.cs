using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;


public class LuaBehaviourManager : Singleton<LuaBehaviourManager>
{
    private LuaTable luaBehaviourManager;

    public Action<Component> EmitStart { get; private set; }
    public Action<Component> EmitEnable { get; private set; }
    public Action<Component> EmitDisable { get; private set; }
    public Action<Component> EmitDestroy { get; private set; }

    public Action<Component, string, UnityEngine.Collision> EmitOnCollision { get; private set; }

    public Action<Component, string, UnityEngine.Collider> EmitOnTrigger { get; private set; }

    public Action<Component, string> Add;

    public LuaBehaviourManager()
    {
        this.luaBehaviourManager = LuaManager.GetMainInstance().LuaClient.Get<LuaTable>("LuaBehaviourManager");

        EmitStart = this.luaBehaviourManager.Get<Action<Component>>("Start");
        EmitEnable = this.luaBehaviourManager.Get<Action<Component>>("OnEnable");
        EmitDisable = this.luaBehaviourManager.Get<Action<Component>>("OnDisable");
        EmitDestroy = this.luaBehaviourManager.Get<Action<Component>>("OnDestroy");

        EmitOnCollision = this.luaBehaviourManager.Get<Action<Component, string, UnityEngine.Collision>>("OnCollision");
        EmitOnTrigger = this.luaBehaviourManager.Get<Action<Component, string, UnityEngine.Collider>>("OnTrigger");

        Add = this.luaBehaviourManager.Get<Action<Component, string>>("Add");
    }
    public override void Dispose()
    {
        EmitStart = null;
        EmitEnable = null;
        EmitDisable = null;
        EmitDestroy = null;
        EmitOnCollision = null;
        EmitOnTrigger = null;
    }

}