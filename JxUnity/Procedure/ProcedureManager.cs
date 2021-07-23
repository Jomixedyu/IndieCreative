using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine;

public class ProcedureManager : Singleton<ProcedureManager>
{
    private FSMBase<string, ProcedureBase> procedure = new FSMBase<string, ProcedureBase>();

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

    public ProcedureManager()
    {
        Type[] types = GetClassTypeByBase(Assembly.GetExecutingAssembly(), typeof(ProcedureBase));
        foreach (Type item in types)
        {
            this.procedure.AddState(item.Name, (ProcedureBase)Activator.CreateInstance(item));
        }
    }

    public void Change(string typeName)
    {
        if (!procedure.HasState(typeName))
        {
            Debug.LogError("not found procedure: " + typeName);
            return;
        }
        Debug.Log("change procedure: " + typeName);
        this.procedure.ChangeState(typeName);
    }
    public void ChangePrevProcedure()
    {
        if(this.procedure.preIndex != null)
        {
            this.Change(this.procedure.preIndex);
        }
    }
    public void Change<TProcedure>() where TProcedure : ProcedureBase
    {
        this.Change(typeof(TProcedure).Name);
    }
    public ProcedureBase GetCurProcedure()
    {
        return this.procedure.GetCurState();
    }
    public void Tick()
    {
        this.procedure.GetCurState()?.OnUpdate();
    }

}

