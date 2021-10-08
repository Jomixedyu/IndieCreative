using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class LuaBehaviour : MonoBehaviour
{
    [SerializeField]
    public string ScriptFullName;

    public LuaBehaviour GetLuaComponent(string name)
    {
        foreach (var item in gameObject.GetComponents<LuaBehaviour>())
        {
            if (item.ScriptFullName == name)
            {
                return item;
            }
        }
        return null;
    }

    private void Awake()
    {
        LuaBehaviourManager.Instance.Add(this, ScriptFullName);
    }
    private void Start()
    {
        LuaBehaviourManager.Instance.EmitStart(this);
    }
    private void OnEnable()
    {
        LuaBehaviourManager.Instance.EmitEnable(this);
    }
    private void OnDisable()
    {
        LuaBehaviourManager.Instance.EmitDisable(this);
    }
    private void OnDestroy()
    {
        LuaBehaviourManager.Instance.EmitDestroy(this);
    }

    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
        LuaBehaviourManager.Instance.EmitOnCollision(this, "OnCollisionEnter", collision);
    }
    private void OnCollisionExit(UnityEngine.Collision collision)
    {
        LuaBehaviourManager.Instance.EmitOnCollision(this, "OnCollisionExit", collision);
    }
    private void OnCollisionStay(UnityEngine.Collision collision)
    {
        LuaBehaviourManager.Instance.EmitOnCollision(this, "OnCollisionStay", collision);
    }
    private void OnTriggerEnter(UnityEngine.Collider other)
    {
        LuaBehaviourManager.Instance.EmitOnTrigger(this, "OnTriggerEnter", other);
    }
    private void OnTriggerExit(UnityEngine.Collider other)
    {
        LuaBehaviourManager.Instance.EmitOnTrigger(this, "OnTriggerExit", other);
    }
    private void OnTriggerStay(UnityEngine.Collider other)
    {
        LuaBehaviourManager.Instance.EmitOnTrigger(this, "OnTriggerStay", other);
    }
}