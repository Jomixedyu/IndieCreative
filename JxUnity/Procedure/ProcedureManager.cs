using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 流程状态管理器
/// </summary>
public class ProcedureManager
{
    private static bool useCustomUpdater = false;

    private static FSM<string, ProcedureBase> procedures;

    public static Type[] GetClassTypeByBase(Assembly ass, Type baseType)
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

    static ProcedureManager()
    {
        procedures = new FSM<string, ProcedureBase>();
        Type[] types = GetClassTypeByBase(Assembly.GetExecutingAssembly(), typeof(ProcedureBase));
        foreach (Type item in types)
        {
            procedures.AddState(item.Name, (ProcedureBase)Activator.CreateInstance(item));
        }
    }

    /// <summary>
    /// 设置后需要手动调用Update函数
    /// </summary>
    public static void UseCustomUpdater()
    {
        useCustomUpdater = true;
        if (ProcedureUpdater.HasInstance())
        {
            ProcedureUpdater.GetInstance().Dispose();
        }
    }
    /// <summary>
    /// 设置后自动调用Update函数
    /// </summary>
    public static void UseDefaultUpdater()
    {
        useCustomUpdater = false;
        ProcedureUpdater.GetInstance();
    }

    /// <summary>
    /// 更改流程状态
    /// </summary>
    /// <param name="typeName"></param>
    public static void Change(string typeName)
    {
        if (!useCustomUpdater)
        {
            ProcedureUpdater.GetInstance();
        }
        if (!procedures.HasState(typeName))
        {
            Debug.LogError("procedure: " + typeName + " not found.");
            return;
        }
        Debug.Log("change procedure: " + typeName);
        procedures.ChangeState(typeName);
    }
    /// <summary>
    /// 更改至上一个流程状态
    /// </summary>
    public static void ChangeToPrevProcedure()
    {
        if (procedures.LastStateIndex != null)
        {
            Change(procedures.LastStateIndex);
        }
    }
    /// <summary>
    /// 更改流程状态
    /// </summary>
    /// <typeparam name="TProcedure"></typeparam>
    public static void Change<TProcedure>() where TProcedure : ProcedureBase
    {
        Change(typeof(TProcedure).Name);
    }
    /// <summary>
    /// 获取当前流程状态
    /// </summary>
    /// <returns></returns>
    public static ProcedureBase GetCurProcedure()
    {
        return procedures.GetCurState();
    }

    public static void Update()
    {
        procedures.GetCurState()?.OnUpdate();
    }

}

