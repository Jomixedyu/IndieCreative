using System.Collections.Generic;
using System;
using System.Reflection;

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
            XLogger.LogError("无法找到Procedure: " + typeName);
            return;
        }
        XLogger.Log("修改Procedure: " + typeName);
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

