using System;
using UnityEngine;


public abstract class JuiBase<UIType> : JuiBaseAbstract where UIType : JuiBase<UIType>, new()
{
    private static UIType mInstance;
    public static UIType Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = Activator.CreateInstance<UIType>();
                mInstance.Create();
            }
            return mInstance;
        }
    }
    public static bool HasInstance
    {
        get => mInstance != null;
    }
    public static UIType GetInstance()
    {
        return Instance;
    }
    public override void Dispose()
    {
        base.Dispose();
        mInstance = null;
    }

}
