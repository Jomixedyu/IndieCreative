using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine;


public abstract class ProcedureBase
{
    public virtual void OnEnter() { }
    public virtual void OnLeave() { }
    public virtual void OnUpdate() { }
}

/// <summary>
/// 流程状态管理器
/// </summary>
public static class ProcedureManager
{
    public static bool HasState(string name)
    {
        return ProcedureManagerMono.Instance.HasState(name);
    }

    public static ProcedureBase GetCurrentState()
    {
        return ProcedureManagerMono.Instance.GetCurrentState();
    }
    public static void Change(string name)
    {
        ProcedureManagerMono.Instance.Change(name);
    }
    public static void Change<T>() where T : ProcedureBase
    {
        ProcedureManagerMono.Instance.Change(typeof(T).Name);
    }

    public static void AddState(Type type)
    {
        ProcedureManagerMono.Instance.AddState(type);
    }

    public static void AddAssembly(Assembly assembly = null)
    {
        if (assembly == null)
        {
            assembly = Assembly.GetCallingAssembly();
        }
        foreach (var type in GetClassTypeByBase(assembly, typeof(ProcedureBase)))
        {
            AddState(type);
        }
    }

    private static Type[] GetClassTypeByBase(Assembly ass, Type baseType)
    {
        Type[] ts = ass.GetTypes();
        List<Type> rst = new List<Type>();
        foreach (Type item in ts)
        {
            if (item.IsSubclassOf(baseType))
            {
                rst.Add(item);
            }
        }
        return rst.ToArray();
    }

}

internal class ProcedureManagerMono : MonoBehaviour
{
    private static ProcedureManagerMono instance;
    public static ProcedureManagerMono Instance
    {
        get
        {
            if (instance == null)
            {
                var go = new GameObject($"__m_ProcedureManagerMono");
                DontDestroyOnLoad(go);
                instance = go.AddComponent<ProcedureManagerMono>();
            }
            return instance;
        }
    }
    public static bool HasInstance => instance != null;

    private Dictionary<string, ProcedureBase> procedures;
    private string curState;
    private string prevState;

    public string CurrentStateName => curState;

    private void Awake()
    {
        this.curState = null;
        this.prevState = null;
        this.procedures = new Dictionary<string, ProcedureBase>();
    }

    public bool HasState(string name)
    {
        return this.procedures.ContainsKey(name);
    }

    public ProcedureBase GetCurrentState()
    {
        if (this.curState == null)
        {
            return null;
        }
        return this.procedures[this.curState];
    }

    public void Change(string name)
    {
        this.prevState = this.curState;
        if (this.curState != null)
        {
            this.procedures[this.curState].OnLeave();
        }
        this.curState = name;
        if (name != null)
        {
            this.procedures[name].OnEnter();
        }
        Debug.Log("ProcedureChanged: " + name);
    }

    public void ChangeToPrev()
    {
        this.Change(this.prevState);
    }

    public void AddState(Type type)
    {
        if (!type.IsSubclassOf(typeof(ProcedureBase)))
        {
            throw new ArgumentException("ProcedureBase");
        }
        this.procedures.Add(type.Name, (ProcedureBase)Activator.CreateInstance(type));
    }

    private void Update()
    {
        this.GetCurrentState()?.OnUpdate();
    }
}