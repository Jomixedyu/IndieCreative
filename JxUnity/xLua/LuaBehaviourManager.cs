using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;


public class LuaBehaviourManager : Singleton<LuaBehaviourManager>
{
    private LuaTable luaBehaviourManager;

    public Action<GameObject> EmitStart { get; private set; }
    public Action<GameObject> EmitEnable { get; private set; }
    public Action<GameObject> EmitDisable { get; private set; }
    public Action<GameObject> EmitDestroy { get; private set; }

    public Action<GameObject, string, Collision> EmitOnCollision { get; private set; }

    public Action<GameObject, string, Collider> EmitOnTrigger { get; private set; }

    public Action<GameObject, MonoBehaviour, string> Add;

    public LuaBehaviourManager()
    {
        this.luaBehaviourManager = LuaClientManager.Instance.LuaClient.Get<LuaTable>("LuaBehaviourManager");

        EmitStart = this.luaBehaviourManager.Get<Action<GameObject>>("Start");
        EmitEnable = this.luaBehaviourManager.Get<Action<GameObject>>("OnEnable");
        EmitDisable = this.luaBehaviourManager.Get<Action<GameObject>>("OnDisable");
        EmitDestroy = this.luaBehaviourManager.Get<Action<GameObject>>("OnDestroy");

        EmitOnCollision = this.luaBehaviourManager.Get<Action<GameObject, string, Collision>>("OnCollision");
        EmitOnTrigger = this.luaBehaviourManager.Get<Action<GameObject, string, Collider>>("OnTrigger");

        Add = this.luaBehaviourManager.Get<Action<GameObject, MonoBehaviour, string>>("Add");
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
