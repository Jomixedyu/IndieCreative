using System;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour, IDisposable where T : MonoSingleton<T>
{
    private static T mInstance = null;

    public static T Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = GameObject.FindObjectOfType(typeof(T)) as T;
                if (mInstance == null)
                {
                    GameObject go = new GameObject(typeof(T).Name);
                    mInstance = go.AddComponent<T>();

                    Debug.LogWarning("New MonoSingleton " + typeof(T));

                    GameObject parent = GameObject.Find("__System");
                    if (parent == null)
                    {
                        parent = new GameObject("__System");
                        DontDestroyOnLoad(parent);
                    }
                    go.transform.parent = parent.transform;
                }
                DontDestroyOnLoad(mInstance.gameObject);
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

    protected bool isDisposed = false;
    public virtual void Dispose()
    {
        if (isDisposed)
        {
            return;
        }
        isDisposed = true;

        mInstance = null;
        if (gameObject != null)
        {
            UnityEngine.Object.Destroy(gameObject);
        }
        Debug.LogWarning("Destroy MonoSingleton: " + typeof(T).Name);
    }
}
