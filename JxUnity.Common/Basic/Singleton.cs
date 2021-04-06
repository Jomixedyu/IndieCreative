using System;

public class Singleton<T> : IDisposable where T : class, new()
{
    private static T mInstance;
    public static T Instance
    {
        get => GetInstance();
    }
    public static bool HasInstance
    {
        get => mInstance != null;
    }
    public static T GetInstance()
    {
        if (mInstance == null)
        {
            mInstance = Activator.CreateInstance<T>();
        }
        return mInstance;
    }

    public virtual void Dispose()
    {
    }
}
