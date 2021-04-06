using System;
using System.Collections.Generic;

public abstract class JuiSingleton<UIType> : JuiBase
{
    private static UIType mInstance;
    public static UIType Instance
    {
        get => GetInstance();
    }
    public static bool HasInstance
    {
        get => mInstance != null;
    }
    public static UIType GetInstance()
    {
        if (mInstance == null)
        {
            mInstance = Activator.CreateInstance<UIType>();
        }
        return mInstance;
    }

}
