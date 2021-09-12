using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class LuaBehaviour : MonoBehaviour
{
    [SerializeField]
    public string ScriptFullName;

    private void Awake()
    {
        LuaBehaviourManager.Instance.Add(gameObject, this, ScriptFullName);
    }
    private void Start()
    {
        LuaBehaviourManager.Instance.EmitStart(gameObject);
    }
    private void OnEnable()
    {
        LuaBehaviourManager.Instance.EmitEnable(gameObject);
    }
    private void OnDisable()
    {
        LuaBehaviourManager.Instance?.EmitDisable(gameObject);
    }
    private void OnDestroy()
    {
        LuaBehaviourManager.Instance?.EmitDestroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        LuaBehaviourManager.Instance.EmitOnCollision(gameObject, "OnCollisionEnter", collision);
    }
    private void OnCollisionExit(Collision collision)
    {
        LuaBehaviourManager.Instance.EmitOnCollision(gameObject, "OnCollisionExit", collision);
    }
    private void OnCollisionStay(Collision collision)
    {
        LuaBehaviourManager.Instance.EmitOnCollision(gameObject, "OnCollisionStay", collision);
    }

    private void OnTriggerEnter(Collider other)
    {
        LuaBehaviourManager.Instance.EmitOnTrigger(gameObject, "OnTriggerEnter", other);
    }
    private void OnTriggerExit(Collider other)
    {
        LuaBehaviourManager.Instance.EmitOnTrigger(gameObject, "OnTriggerExit", other);
    }
    private void OnTriggerStay(Collider other)
    {
        LuaBehaviourManager.Instance.EmitOnTrigger(gameObject, "OnTriggerStay", other);
    }
}
