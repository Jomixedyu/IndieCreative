using System;

public class Singleton<T> : IDisposable where T : class, new()
{
    private static T mInstance;
    public static T Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = Activator.CreateInstance<T>();
            }
            return mInstance;
        }
    }
    public static bool HasInstance
    {
        get => mInstance != null;
    }
    public static T GetInstance()
    {
        return Instance;
    }

    public virtual void Dispose()
    {
    }
}
