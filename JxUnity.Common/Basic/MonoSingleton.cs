using System;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour, IDisposable where T : MonoSingleton<T>
{
    [SerializeField]
    private bool IsDontDestroyOnInit = true;

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

                    //Debug.LogWarning("New MonoSingleton " + typeof(T));

                    GameObject parent = GameObject.Find("__System");
                    if (parent == null)
                    {
                        parent = new GameObject("__System");
                        DontDestroyOnLoad(parent);
                    }
                    go.transform.parent = parent.transform;
                }
                if (mInstance.IsDontDestroyOnInit)
                {
                    if (object.ReferenceEquals(mInstance.transform.parent, null))
                    {
                        DontDestroyOnLoad(mInstance.gameObject);
                    }
                }
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
    protected void SetInstance(T obj)
    {
        mInstance = obj;
    }
    /// <summary>
    /// 检查单例实例是否存在，如果存在则销毁
    /// </summary>
    /// <returns></returns>
    protected bool CheckInstanceAndDestroy()
    {
        if (HasInstance)
        {
            Destroy(gameObject);
            return true;
        }
        return false;
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
        //Debug.LogWarning("Destroy MonoSingleton: " + typeof(T).Name);
    }
}
