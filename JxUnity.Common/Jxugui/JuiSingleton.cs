using System;

public abstract class JuiSingleton<UIType> : JuiBase where UIType : class
{
    private static UIType mInstance;
    public static UIType Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = Activator.CreateInstance<UIType>();
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
